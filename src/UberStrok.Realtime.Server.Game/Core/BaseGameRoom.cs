using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UberStrok.Core;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Game
{
    public abstract partial class BaseGameRoom : BaseGameRoomOperationHandler, IRoom<GamePeer>, IDisposable
    {
        private bool _disposed;
        private byte _nextPlayer;
        private string _password;

        /* List of peers connected to the game room (not necessarily playing). */
        private readonly List<GamePeer> _peers;
        /* List of peers connected & playing. */
        private readonly List<GamePeer> _players;

        private readonly List<GamePeer> _failedPeers;

        protected ILog Log { get; }
        protected ILog ReportLog { get; }

        public Loop Loop { get; }
        public GameRoomDataView View { get; }
        public IReadOnlyList<GamePeer> Peers { get; }
        public IReadOnlyList<GamePeer> Players { get; }
        public StateMachine<MatchState.Id> State { get; }
        public ShopManager Shop { get; }
        public SpawnManager Spawns { get; }
        public PowerUpManager PowerUps { get; }

        public event EventHandler<PlayerKilledEventArgs> PlayerKilled;
        public event EventHandler<PlayerRespawnedEventArgs> PlayerRespawned;
        public event EventHandler<PlayerJoinedEventArgs> PlayerJoined;
        public event EventHandler<PlayerLeftEventArgs> PlayerLeft;

        public int RoundNumber { get; set; }
        /* Time in system ticks when the round ends.*/
        public int EndTime { get; set; }

        public bool IsRunning => State.Current == MatchState.Id.Running;
        public bool IsWaitingForPlayers => State.Current == MatchState.Id.WaitingForPlayers;

        /* Room ID but we call it number since we already defined Id & thats how UberStrike calls it too. */
        public int Number
        {
            get => View.Number;
            set => View.Number = value;
        }

        public string Password
        {
            get => _password;
            set
            {
                /* If the password is null or empty it means its not password protected. */
                View.IsPasswordProtected = !string.IsNullOrEmpty(value);
                _password = value;
            }
        }

        public BaseGameRoom(GameRoomDataView data)
        {
            View = data ?? throw new ArgumentNullException(nameof(data));
            View.ConnectedPlayers = 0;

            Log = LogManager.GetLogger(GetType().Name);
            ReportLog = LogManager.GetLogger("Report");

            _peers = new List<GamePeer>();
            _players = new List<GamePeer>();
            _failedPeers = new List<GamePeer>();

            Peers = _peers.AsReadOnly();
            Players = _players.AsReadOnly();

            /* Using a high tick rate to push updates to the client faster. */
            Loop = new Loop(64);

            Shop = new ShopManager();
            Spawns = new SpawnManager();
            PowerUps = new PowerUpManager(this);

            State = new StateMachine<MatchState.Id>();
            State.Register(MatchState.Id.None, null);
            State.Register(MatchState.Id.WaitingForPlayers, new WaitingForPlayersMatchState(this));
            State.Register(MatchState.Id.Countdown, new CountdownMatchState(this));
            State.Register(MatchState.Id.Running, new RunningMatchState(this));

            State.Set(MatchState.Id.WaitingForPlayers);

            /* Start the game room loop. */
            Loop.Start(OnTick, OnTickFailed);
        }

        public void Join(GamePeer peer)
        {
            if (peer == null)
                throw new ArgumentNullException(nameof(peer));

            Debug.Assert(peer.Room == null, "GamePeer is joining room, but its already in another room.");

            Enqueue(() => {
                Log.Info("Peer joining room");
                try
                {
                    /* 
                     * If a peer with the same cmid is somehow in the room, 
                     * disconnect him.
                     */
                    foreach (var otherPeer in Peers)
                    {
                        if (otherPeer.Actor.Cmid == peer.Member.CmuneMemberView.PublicProfile.Cmid)
                        {
                            Leave(otherPeer);
                            break;
                        }
                    }

                    var publicProfile = peer.Member.CmuneMemberView.PublicProfile;
                    var actorView = new GameActorInfoView
                    {
                        Channel = ChannelType.Steam,
                        PlayerState = PlayerStates.None,
                        TeamID = TeamID.NONE,
                        Health = 100,
                        ArmorPointCapacity = 0,
                        ArmorPoints = 0,
                        Deaths = 0,
                        Kills = 0,
                        Level = 1,
                        Ping = (ushort)(peer.RoundTripTime / 2),

                        Cmid = publicProfile.Cmid,
                        ClanTag = publicProfile.GroupTag,
                        AccessLevel = publicProfile.AccessLevel,
                        PlayerName = publicProfile.Name,
                    };

                    /* Set the gears of the character. */
                    /* Holo */
                    actorView.Gear[0] = peer.Loadout.Webbing;
                    actorView.Gear[1] = peer.Loadout.Head;
                    actorView.Gear[2] = peer.Loadout.Face;
                    actorView.Gear[3] = peer.Loadout.Gloves;
                    actorView.Gear[4] = peer.Loadout.UpperBody;
                    actorView.Gear[5] = peer.Loadout.LowerBody;
                    actorView.Gear[6] = peer.Loadout.Boots;

                    /* Sets the weapons of the character. */
                    actorView.Weapons[0] = peer.Loadout.MeleeWeapon;
                    actorView.Weapons[1] = peer.Loadout.Weapon1;
                    actorView.Weapons[2] = peer.Loadout.Weapon2;
                    actorView.Weapons[3] = peer.Loadout.Weapon3;

                    /* Set Quick items of the character. */
                    actorView.QuickItems[0] = peer.Loadout.QuickItem1;
                    actorView.QuickItems[0] = peer.Loadout.QuickItem2;
                    actorView.QuickItems[0] = peer.Loadout.QuickItem3;

                    var number = 0;
                    var actor = new GameActor(actorView);

                    /* TODO: Check for possible overflows. */
                    number = _nextPlayer++;

                    peer.Room = this;
                    peer.Actor = actor;
                    peer.Actor.Number = number;

                    var weaponViews = new List<UberStrikeItemWeaponView>();
                    foreach (var itemId in actorView.Weapons)
                    {
                        if (itemId == 0)
                            continue;

                        weaponViews.Add(Shop.WeaponItems[itemId]);
                    }

                    peer.Actor.Weapons.Update(weaponViews);
                    peer.Actor.Ping.Reset();
                    peer.UpdateArmorCapacity();
                    peer.Handlers.Add(this);

                    /* 
                     * This prepares the client for the game room and sets the client
                     * state to 'pre-game'.
                     */
                    peer.Events.SendRoomEntered(View);

                    /* 
                     * Set the player in the overview state. Which also sends all player
                     * data in the room to the peer.
                     */
                    peer.State.Set(PeerState.Id.Overview);

                    Log.Info("Set peer state to Overview");

                    _peers.Add(peer);
                }
                catch (Exception ex)
                {
                    Leave(peer);
                    peer.Events.SendRoomEnterFailed(string.Empty, 0, "Failed to join room.");
                    Log.Error("Failed to join room", ex);
                }
            });
        }

        public void Leave(GamePeer peer)
        {
            if (peer == null)
                throw new ArgumentNullException(nameof(peer));

            Debug.Assert(peer.Room != null, "GamePeer is leaving room, but its not in a room.");
            Debug.Assert(peer.Room == this, "GamePeer is leaving room, but its not leaving the correct room.");

            Enqueue(() => {
                /* Let other peers know that the peer has left the room. */
                foreach (var otherPeer in Peers)
                {
                    otherPeer.Events.Game.SendPlayerLeftGame(peer.Actor.Cmid);
                    otherPeer.KnownActors.Remove(peer.Actor.Cmid);
                }

                _peers.Remove(peer);
                _players.Remove(peer);

                View.ConnectedPlayers = Players.Count;

                /* Set peer state to none, and clean up. */
                peer.State.Set(PeerState.Id.None);
                peer.Handlers.Remove(Id);
                peer.KnownActors.Clear();
                peer.Actor = null;
                peer.Room = null;
            });
        }

        private void OnTick()
        {
            State.Update();

            _failedPeers.Clear();
            foreach (var peer in Peers)
            {
                if (peer.HasError)
                {
                    peer.Disconnect();
                    break;
                }

                try { peer.Update(); }
                catch (Exception ex)
                {
                    /* NOTE: This should never happen, but just incase. */
                    Log.Error("Failed to update peer.", ex);
                    _failedPeers.Add(peer);
                }
            }

            foreach (var peer in _failedPeers)
                Leave(peer);
        }

        private void OnTickFailed(Exception e)
        {
            Log.Error("Failed to tick game loop.", e);
        }

        protected abstract bool CanDamage(GamePeer victim, GamePeer attacker);

        /* Does damage and returns true if victim is killed; otherwise false. */
        protected bool DoDamage(GamePeer victim, GamePeer attacker, short damage, BodyPart part, out Vector3 direction)
        {
            bool selfDamage = victim.Actor.Cmid == attacker.Actor.Cmid;

            /* Calculate the direction of the hit. */
            var victimPos = victim.Actor.Movement.Position;
            var attackerPos = attacker.Actor.Movement.Position;
            direction = attackerPos - victimPos;

            if ((victim.Actor.Info.PlayerState & PlayerStates.Dead) == PlayerStates.Dead)
                return false;

            /* Check if we can apply the damage on the players. */
            if (!CanDamage(victim, attacker))
                return false;

            float angle = Vector3.Angle(direction, new Vector3(0, 0, -1));
            if (direction.x < 0)
                angle = 360 - angle;

            byte byteAngle = Conversions.Angle2Byte(angle);

            /* Check if not self-damage. */
            if (!selfDamage)
                victim.Actor.Damages.Add(byteAngle, damage, part, 0, 0);
            else
                damage /= 2;

            /* Calculate armor absorption. */
            int armorDamage;
            int healthDamage;
            if (victim.Actor.Info.ArmorPoints > 0)
            {
                armorDamage = (byte)(0.66f * damage);
                healthDamage = (short)(damage - armorDamage);
            }
            else
            {
                armorDamage = 0;
                healthDamage = damage;
            }

            int newArmor = victim.Actor.Info.ArmorPoints - armorDamage;
            int newHealth = victim.Actor.Info.Health - healthDamage;

            if (newArmor < 0)
                newHealth += newArmor;

            victim.Actor.Info.ArmorPoints = (byte)Math.Max(0, newArmor);
            victim.Actor.Info.Health = (short)Math.Max(0, newHealth);

            /* Check if the player is dead. */
            if (victim.Actor.Info.Health <= 0)
            {
                if (victim.Actor.Damages.Count > 0)
                {
                    /* 
                     * Force a push of damage events to the victim peer, so he
                     * gets the feedback of where he was hit from aka red hit
                     * marker HUD.
                     */
                    victim.Events.Game.SendDamageEvent(victim.Actor.Damages);
                    victim.Flush();
                    victim.Actor.Damages.Clear();
                }

                victim.Actor.Info.PlayerState |= PlayerStates.Dead;

                victim.Actor.Info.Deaths++;
                if (selfDamage)
                    attacker.Actor.Info.Kills--;
                else
                    attacker.Actor.Info.Kills++;

                victim.State.Set(PeerState.Id.Killed);
                return true;
            }

            return false;
        }

        protected virtual void OnPlayerRespawned(PlayerRespawnedEventArgs args)
        {
            PlayerRespawned?.Invoke(this, args);
        }

        protected virtual void OnPlayerJoined(PlayerJoinedEventArgs args)
        {
            PlayerJoined?.Invoke(this, args);
        }

        protected virtual void OnPlayerLeft(PlayerLeftEventArgs args)
        {
            PlayerLeft?.Invoke(this, args);
        }

        protected virtual void OnPlayerKilled(PlayerKilledEventArgs args)
        {
            PlayerKilled?.Invoke(this, args);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
                Loop.Dispose();

            _disposed = true;
        }
    }
}
