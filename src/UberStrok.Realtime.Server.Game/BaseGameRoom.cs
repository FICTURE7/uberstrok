using log4net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Game
{
    public class PlayerRespawnedEventArgs : EventArgs
    {
        public GamePeer Player { get; set; }
    }

    public class PlayerJoinedEventArgs : EventArgs
    {
        public GamePeer Player { get; set; }
        public TeamID Team { get; set; }
    }

    public class PlayerLeftEventArgs : EventArgs
    {
        public GamePeer Player { get; set; }
    }

    public abstract partial class BaseGameRoom : BaseGameRoomOperationHandler, IRoom<GamePeer>
    {
        private readonly static ILog s_log = LogManager.GetLogger(nameof(BaseGameRoom));

        private StateMachine<MatchState.Id> _state;

        /* Where the heavy lifting is done. */
        private Thread _loopThread;
        /* Figure out if the loop has started.*/
        private bool _loopStarted;

        private byte _nextPlayer;

        /* Password of the room. */
        private string _password;

        /* Data of the game room. */
        private readonly GameRoomDataView _data;

        /* List of peers connected to the game room (not necessarily playing). */
        private readonly List<GamePeer> _peers;
        /* List of peers connected & playing. */
        private readonly List<GamePeer> _players;
        
        /* Manages items, mostly to get weapon damage. */
        private readonly ShopManager _shopManager;
        /* Manages the power ups. */
        private readonly PowerUpManager _powerUpManager;
        /* Manages the spawn of players. */
        private readonly SpawnManager _spawnManager;

        /* Keep refs to ReadOnlyCollections to be a little bit GC friendly. */
        private readonly IReadOnlyList<GamePeer> _peersReadOnly;
        private readonly IReadOnlyList<GamePeer> _playersReadonly;

        public BaseGameRoom(GameRoomDataView data)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            if (data.GameMode != GameModeType.TeamDeathMatch)
                throw new ArgumentException("GameRoomDataView is not in team deathmatch mode.", nameof(data));

            _data = data;
            _data.ConnectedPlayers = 0;

            _peers = new List<GamePeer>();
            _players = new List<GamePeer>();

            _shopManager = new ShopManager();
            _spawnManager = new SpawnManager();
            _powerUpManager = new PowerUpManager();

            _peersReadOnly = _peers.AsReadOnly();
            _playersReadonly = _players.AsReadOnly();

            _state = new StateMachine<MatchState.Id>();
            _state.Register(MatchState.Id.None, null);
            _state.Register(MatchState.Id.WaitingForPlayers, new WaitingForPlayersMatchState(this));
            _state.Register(MatchState.Id.Countdown, new CountdownMatchState(this));
            _state.Register(MatchState.Id.Running, new RunningMatchState(this));

            _state.Set(MatchState.Id.WaitingForPlayers);
        }

        public override int Id => 0;
        public GameRoomDataView Data => _data;
        public IReadOnlyList<GamePeer> Peers => _peersReadOnly;
        public IReadOnlyList<GamePeer> Players => _playersReadonly;
        public StateMachine<MatchState.Id> State => _state;

        public event EventHandler<PlayerKilledEventArgs> PlayerKilled;
        public event EventHandler<PlayerRespawnedEventArgs> PlayerRespawned;
        public event EventHandler<PlayerJoinedEventArgs> PlayerJoined;
        public event EventHandler<PlayerLeftEventArgs> PlayerLeft;

        /* Room ID but we call it number since we already defined Id & thats how UberStrike calls it too. */
        public int Number
        {
            get { return _data.Number; }
            set { _data.Number = value; }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                /* If the password is null or empty it means its not password protected. */
                _data.IsPasswordProtected = !string.IsNullOrEmpty(value);
                _password = value;
            }
        }

        /* Round number. */
        public int RoundNumber { get; set; }
        /* Time in system ticks when the round ends.*/
        public int EndTime { get; set; }

        public ShopManager ShopManager => _shopManager;
        public SpawnManager SpawnManager => _spawnManager;

        public bool IsStarted => State.Current == MatchState.Id.Running;
        public bool IsWaitingForPlayers => State.Current == MatchState.Id.WaitingForPlayers;

        public void Join(GamePeer peer)
        {
            if (peer == null)
                throw new ArgumentNullException(nameof(peer));

            Debug.Assert(peer.Room == null, "GamePeer is joining room, but its already in another room.");

            var data = new GameActorInfoView
            {
                TeamID = TeamID.NONE,

                Health = 100,

                //TODO: Calculate armor points & armor capacity (but who cares about those).
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
            data.Gear[0] = (int)peer.Loadout.Type;
            data.Gear[1] = peer.Loadout.Head;
            data.Gear[2] = peer.Loadout.Face;
            data.Gear[3] = peer.Loadout.Gloves;
            data.Gear[4] = peer.Loadout.UpperBody;
            data.Gear[5] = peer.Loadout.LowerBody;
            data.Gear[6] = peer.Loadout.Boots;

            /* Sets the weapons of the character. */
            data.Weapons[0] = peer.Loadout.MeleeWeapon;
            data.Weapons[1] = peer.Loadout.Weapon1;
            data.Weapons[2] = peer.Loadout.Weapon2;
            data.Weapons[3] = peer.Loadout.Weapon3;

            var number = 0;
            var actor = new GameActor(data);

            lock (_peers)
            {
                _peers.Add(peer);
                number = _nextPlayer++;
            }

            peer.Room = this;
            peer.Actor = actor;
            peer.Actor.Number = number;
            peer.AddOperationHandler(this);

            /* 
                This prepares the client for the game room and
                sets the client state to 'pre-game'.
             */
            peer.Events.SendRoomEntered(Data);

            /* 
                Set the player in the overview state. Which
                also sends all player data in the room.
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
            foreach (var otherPeer in Peers)
            {
                otherPeer.Events.Game.SendPlayerLeftGame(peer.Actor.Cmid);
                otherPeer.KnownActors.Remove(peer.Actor.Cmid);
            }

            lock (_peers)
            {
                _peers.Remove(peer);
                _players.Remove(peer);

                _data.ConnectedPlayers = Players.Count;
            }

            /* Set peer state to none, and clean up. */
            peer.State.Set(PeerState.Id.None);
            peer.RemoveOperationHandler(Id);
            peer.KnownActors.Clear();
            peer.Actor = null;
            peer.Room = null;
        }

        public void StartLoop()
        {
            /*
                Kill the previous loop thread if it exists and
                start a new loop thread.
             */
            if (_loopThread != null)
                _loopThread.Abort();

            _loopStarted = true;

            _loopThread = new Thread(GameLoop);
            _loopThread.Start();
        }

        private void GameLoop()
        {
            const int TICK_RATE = 64;
            const int SLEEP = 1000 / TICK_RATE;

            //TODO: Make the loop fancier using catch-up stuffz & tings.
            //TODO: Fix potential threading issues.
            try
            {
                while (_loopStarted)
                {
                    try { State.Update(); }
                    catch (Exception ex) { s_log.Error("Failed to tick game loop", ex); }

                    Thread.Sleep(SLEEP);
                }

                s_log.Debug("Game has stopped!");
            }
            catch (ThreadAbortException)
            {
                s_log.Debug("Loop thread was aborted!");
            }
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
    }
}
