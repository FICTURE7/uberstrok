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
        private readonly static ILog s_log = LogManager.GetLogger(nameof(BaseGameRoom));

        private bool _disposed;
        private byte _nextPlayer;
        private string _password;

        /* List of peers connected to the game room (not necessarily playing). */
        private readonly List<GamePeer> _peers;
        /* List of peers connected & playing. */
        private readonly List<GamePeer> _players;

        /* Object to synchronize access to the room. */
        public object Sync { get; }

        public GameRoomDataView View { get; }
        public IReadOnlyList<GamePeer> Peers { get; }
        public IReadOnlyList<GamePeer> Players { get; }
        public StateMachine<MatchState.Id> State { get; }
        public PowerUpManager PowerUps { get; }
        public GameRoomActions Actions { get; }

        public event EventHandler<PlayerKilledEventArgs> PlayerKilled;
        public event EventHandler<PlayerRespawnedEventArgs> PlayerRespawned;
        public event EventHandler<PlayerJoinedEventArgs> PlayerJoined;
        public event EventHandler<PlayerLeftEventArgs> PlayerLeft;

        public Loop Loop { get; }
        public ShopManager ShopManager { get; }
        public SpawnManager SpawnManager { get; }

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

            _peers = new List<GamePeer>();
            _players = new List<GamePeer>();

            Peers = _peers.AsReadOnly();
            Players = _players.AsReadOnly();

            Sync = new object();

            /* Using a high tick rate to push updates to the client faster. */
            Loop = new Loop(64);
            Actions = new GameRoomActions(this);

            ShopManager = new ShopManager();
            SpawnManager = new SpawnManager();
            PowerUps = new PowerUpManager(this);

            State = new StateMachine<MatchState.Id>();
            State.Register(MatchState.Id.None, null);
            State.Register(MatchState.Id.WaitingForPlayers, new WaitingForPlayersMatchState(this));
            State.Register(MatchState.Id.Countdown, new CountdownMatchState(this));
            State.Register(MatchState.Id.Running, new RunningMatchState(this));

            State.Set(MatchState.Id.WaitingForPlayers);

            /* Start the game loop ASAP. */
            StartLoop();
        }

        public void Join(GamePeer peer)
        {
            if (peer == null)
                throw new ArgumentNullException(nameof(peer));

            Debug.Assert(peer.Room == null, "GamePeer is joining room, but its already in another room.");

            /* 
             * If a peer with the same cmid is somehow in the room, disconnect him.
             * Avoiding a dead-lock by calling Leave outside the lock.
             */
            var peerAlreadyConnected = default(GamePeer);
            lock (Sync)
            {
                foreach (var otherPeer in _peers)
                {
                    if (otherPeer.Actor.Cmid == peer.Member.CmuneMemberView.PublicProfile.Cmid)
                    {
                        peerAlreadyConnected = otherPeer;
                        break;
                    }
                }
            }

            if (peerAlreadyConnected != null)
                Leave(peerAlreadyConnected);

            var roomView = View;
            var actorView = new GameActorInfoView
            {
                TeamID = TeamID.NONE,

                Health = 100,

                /* 
                 * TODO: Calculate armor points & armor capacity (but who cares
                 * about those).
                 */
                ArmorPoints = 0,
                ArmorPointCapacity = 0,

                Deaths = 0,
                Kills = 0,

                Level = 1,

                Channel = ChannelType.Steam,
                PlayerState = PlayerStates.None,

                Ping = (ushort)(peer.RoundTripTime / 2),

                Cmid = peer.Member.CmuneMemberView.PublicProfile.Cmid,
                ClanTag = peer.Member.CmuneMemberView.PublicProfile.GroupTag,
                AccessLevel = peer.Member.CmuneMemberView.PublicProfile.AccessLevel,
                PlayerName = peer.Member.CmuneMemberView.PublicProfile.Name,
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

            lock (Sync)
            {
                _peers.Add(peer);
                /* TODO: Check for possible overflows. */
                number = _nextPlayer++;
            }

            peer.Room = this;
            peer.Actor = actor;
            peer.Actor.Number = number;
            peer.Handlers.Add(this);

            /* 
             * This prepares the client for the game room and sets the client
             * state to 'pre-game'.
             */
            peer.Events.SendRoomEntered(roomView);

            /* 
             * Set the player in the overview state. Which also sends all player
             * data in the room to the peer.
             */
            peer.State.Set(PeerState.Id.Overview);
        }

        public void Leave(GamePeer peer)
        {
            if (peer == null)
                throw new ArgumentNullException(nameof(peer));

            Debug.Assert(peer.Room != null, "GamePeer is leaving room, but its not in a room.");
            Debug.Assert(peer.Room == this, "GamePeer is leaving room, but its not leaving the correct room.");

            /* Let other peers know that the peer has left the room. */
            Actions.PlayerLeft(peer);

            lock (Sync)
            {
                _peers.Remove(peer);
                _players.Remove(peer);

                View.ConnectedPlayers = Players.Count;
            }

            /* Set peer state to none, and clean up. */
            peer.State.Set(PeerState.Id.None);
            peer.Handlers.Remove(Id);
            peer.KnownActors.Clear();
            peer.Actor = null;
            peer.Room = null;
        }

        public void StartLoop()
        {
            Loop.Start(
                () => 
                {
                    lock (Sync)
                    {
                        foreach (var peer in Peers)
                        {
                            if (peer.HasError) peer.Disconnect();
                            else peer.Update();
                        }
                    }

                    State.Update();
                },
                (ex) => s_log.Error("Failed to tick game loop.", ex)
            );
        }

        /* Does damage and returns true if victim is dead; otherwise false. */
        private bool DoDamage(short damage, BodyPart part, GamePeer victim, GamePeer attacker, out Vector3 direction)
        {
            bool selfDamage = victim.Actor.Cmid == attacker.Actor.Cmid;

            /* Calculate the direction of the hit. */
            var victimPos = victim.Actor.Movement.Position;
            var attackerPos = attacker.Actor.Movement.Position;
            direction = attackerPos - victimPos;

            var angle = Vector3.Angle(direction, new Vector3(0, 0, -1));
            if (direction.x < 0)
                angle = 360 - angle;

            var byteAngle = Conversions.Angle2Byte(angle);

            /* Check if not self-damage. */
            if (!selfDamage)
            {
                victim.Actor.Damages.Add(byteAngle, damage, part, 0, 0);
                victim.Actor.Info.Health -= damage;
            }
            else
            {
                victim.Actor.Info.Health -= (short)(damage / 2);
            }

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
