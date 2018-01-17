using log4net;
using PhotonHostRuntimeInterfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Game
{
    public abstract class BaseGameRoom : BaseGameRoomOperationHandler, IRoom<GamePeer>
    {
        private readonly static ILog s_log = LogManager.GetLogger(nameof(BaseGameRoom));

        /* Where the heavy lifting is done. */
        private Thread _loopThread;
        /* Figure out if the loop has started.*/
        private bool _loopStarted;

        /* Determine wether the game has started. */
        private bool _started;
        /* Determine wether to send countdown and stuff. */
        private bool _countDown;
        /* When the count down ends. */
        private TimeSpan _countDownDuration;

        /* Time in system ticks when the round ends.*/
        private int _endTime;
        /* Round number. */
        private int _roundNumber;

        /* Password of the room. */
        private string _password;

        /* Data of the game room. */
        private readonly GameRoomDataView _data;

        /* List of peers connected to the game room (not necessarily playing). */
        private readonly List<GamePeer> _peers;
        /* List of peers connected & playing. */
        private readonly List<GamePeer> _players;

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
            _spawnManager = new SpawnManager();
            _powerUpManager = new PowerUpManager();

            _peersReadOnly = _peers.AsReadOnly();
            _playersReadonly = _players.AsReadOnly();
        }

        public override int Id => 0;
        public GameRoomDataView Data => _data;
        public IReadOnlyList<GamePeer> Peers => _peersReadOnly;
        public IReadOnlyList<GamePeer> Players => _playersReadonly;

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

        public void Join(GamePeer peer)
        {
            if (peer == null)
                throw new ArgumentNullException(nameof(peer));

            Debug.Assert(peer.Room == null, "GamePeer is joining room, but its already in another room.");

            const int GEAR_COUNT = 7;
            const int WEAPON_COUNT = 4;

            /*
                Create the the gear list and weapons list.
                The client does not like it when its not of the proper size.
             */
            var gear = new List<int>(GEAR_COUNT);
            var weapons = new List<int>(peer.Member.CmuneMemberView.MemberItems);

            for (int i = 0; i < GEAR_COUNT; i++)
                gear.Add(0);

            var weaponCount = peer.Member.CmuneMemberView.MemberItems.Count;
            for (int i = 0; i < WEAPON_COUNT - weaponCount; i++)
                weapons.Add(0);

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

                Weapons = weapons,
                Gear = gear
            };

            var number = 0;
            var actor = new GameActor(data);

            lock (_peers)
            {
                _peers.Add(peer);
                number = _peers.Count;
            }

            peer.Room = this;
            peer.Actor = actor;
            peer.Actor.Number = number;
            peer.AddOperationHandler(this);

            /* 
                This prepares the client for the game room and
                sets the client state to pre-game.
             */
            peer.Events.SendRoomEntered(Data);

            /* Let the client know about the other peers in the room, if there is any.*/
            if (Players.Count > 0)
            {
                var allPlayers = new List<GameActorInfoView>(Peers.Count);
                var allPositions = new List<PlayerMovement>(Peers.Count);
                foreach (var player in Players)
                {
                    allPlayers.Add(player.Actor.Data);
                    allPositions.Add(player.Actor.Movement);
                }

                peer.Events.Game.SendAllPlayers(allPlayers, allPositions, 0);
            }
        }

        public void Leave(GamePeer peer)
        {
            if (peer == null)
                throw new ArgumentNullException(nameof(peer));

            Debug.Assert(peer.Room != null, "GamePeer is leaving room, but its not in a room.");
            Debug.Assert(peer.Room == this, "GamePeer is leaving room, but its not leaving the correct room.");

            lock (_peers)
            {
                _peers.Remove(peer);
                _players.Remove(peer);
                _data.ConnectedPlayers = Players.Count;
            }

            /* Let other peers know that the peer has left the room. */
            foreach (var otherPeer in Peers)
                otherPeer.Events.Game.SendPlayerLeftGame(peer.Actor.Cmid);

            /* If we have 0 players, we stop the game so we can start a new one later on. */
            if (Players.Count == 0)
            {
                _loopStarted = false;
                _started = false;
            }

            peer.RemoveOperationHandler(Id);
            peer.Actor = null;
            peer.Room = null;
        }

        public override void OnDisconnect(GamePeer peer, DisconnectReason reasonCode, string reasonDetail)
        {
            Leave(peer);
        }

        private void StartMatch()
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

            _countDown = true;
            _countDownDuration = TimeSpan.FromSeconds(5);

            /* Calculate the time when the games ends (in system ticks). */
            _endTime = Environment.TickCount + Data.TimeLimit * 1000;

            s_log.Debug($"Started new match: {_roundNumber} which should end at system tick: {_endTime}.");

            foreach (var player in Players)
            {
                var point = _spawnManager.Get(player.Actor.Team);
                var movement = player.Actor.Movement;
                movement.Position = point.Position;
                movement.HorizontalRotation = point.Rotation;

                /*
                    This prepares the client for the next round and enables match start
                    countdown thingy.
                 */
                player.Events.Game.SendPrepareNextRound();

                /* 
                    MatchStart event changes the match state of the client to match running,
                    which in turn changes the player state to playing.

                    The client does not care about the roundNumber apparently (in TeamDeatchMatch atleast).
                 */
                //player.Events.Game.SendMatchStart(_roundNumber, _endTime);

                /* Let all peers know that the player has joined the game. */
                foreach (var otherPeer in Peers)
                {
                    /*
                        PlayerJoinedGame event tells the client to initiate the character and register it
                        in its player list and update the team player number counts.
                     */
                    otherPeer.Events.Game.SendPlayerJoinedGame(player.Actor.Data, movement);
                    otherPeer.Events.Game.SendPlayerRespawned(player.Actor.Cmid, movement.Position, movement.HorizontalRotation);
                }

                //player.Events.Game.SendPlayerRespawned(player.Actor.Cmid, movement.Position, movement.HorizontalRotation);

                s_log.Debug($"Spawned: {player.Actor.Cmid} at: {point}");
            }

            _started = true;
            _roundNumber++;
        }

        protected override void OnJoinGame(GamePeer peer, TeamID team)
        {
            /* 
                When the client joins a game it resets its game state to 'none'.               

                Update the actor's team + other data and register the peer in the player list.
                Update the number of connected players while we're at it.
             */
            peer.Actor.Team = team;
            peer.Actor.Data.Health = 100;
            peer.Actor.Data.Ping = (ushort)(peer.RoundTripTime / 2);
            peer.Actor.Data.PlayerState = PlayerStates.Ready;

            lock (_peers)
            {
                _players.Add(peer);
                _data.ConnectedPlayers = Players.Count;
            }

            s_log.Info($"Joining team -> CMID:{peer.Actor.Cmid}:{team}:{peer.Actor.Number}");

            /*
                If the match has not started yet and there is more
                than 1 players, we start the match.
             */
            if (!_started && Players.Count > 1)
            {
                StartMatch();
                return;
            }

            /*
                If we haven't yet started the match we send the peer
                in 'waiting for players' state.
             */
            if (!_started)
            {
                /* Let all peers know that the client has joined. */
                foreach (var otherPeer in Peers)
                    otherPeer.Events.Game.SendPlayerJoinedGame(peer.Actor.Data, peer.Actor.Movement);

                peer.Events.Game.SendWaitingForPlayer();
            }
            /*
                Otherwise we send the client to random spawn for its
                team.
             */
            else
            {
                var point = _spawnManager.Get(peer.Actor.Team);
                peer.Actor.Movement.Position = point.Position;
                peer.Actor.Movement.HorizontalRotation = point.Rotation;

                peer.Events.Game.SendMatchStart(_roundNumber, _endTime);

                /* Let all peers know that the client has joined. */
                foreach (var otherPeer in Peers)
                {
                    if (otherPeer.Actor.Cmid != peer.Actor.Cmid)
                        otherPeer.Events.Game.SendPlayerJoinedGame(peer.Actor.Data, peer.Actor.Movement);
                }

                peer.Events.Game.SendPlayerJoinedGame(peer.Actor.Data, peer.Actor.Movement);
                peer.Events.Game.SendPlayerRespawned(peer.Actor.Cmid, point.Position, point.Rotation);
            }
        }

        protected override void OnChatMessage(GamePeer peer, string message, ChatContext context)
        {
            var cmid = peer.Actor.Cmid;
            var playerName = peer.Actor.PlayerName;
            var accessLevel = peer.Actor.AccessLevel;

            foreach (var otherPeer in Peers)
            {
                if (otherPeer.Actor.Cmid != cmid)
                    otherPeer.Events.Game.SendChatMessage(cmid, playerName, message, accessLevel, context);
            }
        }

        protected override void OnPowerUpRespawnTimes(GamePeer peer, List<ushort> respawnTimes)
        {
            /* We care only about the first operation sent. */
            if (!_powerUpManager.IsLoaded())
                _powerUpManager.Load(respawnTimes);
        }

        protected override void OnSpawnPositions(GamePeer peer, TeamID team, List<Vector3> positions, List<byte> rotations)
        {
            /* We care only about the first operation sent for that team ID. */
            if (!_spawnManager.IsLoaded(team))
                _spawnManager.Load(team, positions, rotations);
        }

        protected override void OnIsFiring(GamePeer peer, bool on)
        {
            // Space
        }

        protected override void OnJump(GamePeer peer, Vector3 position)
        {
            foreach (var otherPeer in Peers)
            {
                if (otherPeer.Actor.Cmid != peer.Actor.Cmid)
                    otherPeer.Events.Game.SendPlayerJumped(peer.Actor.Cmid, peer.Actor.Movement.Position);
            }
        }

        protected override void OnUpdatePositionAndRotation(GamePeer peer, Vector3 position, Vector3 velocity, byte horizontalRotation, byte verticalRotation, byte moveState)
        {
            peer.Actor.Movement.Position = position;
            peer.Actor.Movement.Velocity = velocity;
            peer.Actor.Movement.HorizontalRotation = horizontalRotation;
            peer.Actor.Movement.VerticalRotation = verticalRotation;
            peer.Actor.Movement.MovementState = moveState;
        }

        protected override void OnSwitchWeapon(GamePeer peer, byte slot)
        {
            peer.Actor.Data.CurrentWeaponSlot = slot;
        }

        protected override void OnIsPaued(GamePeer peer, bool on)
        {
            if (on)
                peer.Actor.Data.PlayerState |= PlayerStates.Paused;
            else
                peer.Actor.Data.PlayerState &= ~PlayerStates.Paused;
        }

        private void GameLoop()
        {
            /* Time when the next ping update happens. */
            var nextPingUpdate = DateTime.UtcNow.AddSeconds(1);
            var oldCountDownDuration = _countDownDuration;

            const int TICK_RATE = 64;
            const int SLEEP = 1000 / TICK_RATE;

            //TODO: Make the loop fancier using catch-up stuffz & tings.
            //TODO: Fix potential threading issues.
            try
            {
                while (_loopStarted)
                {
                    /* Wait until everything is set up. */
                    if (!_started)
                        continue;

                    try
                    {
                        if (_countDown)
                        {
                            oldCountDownDuration = _countDownDuration;
                            _countDownDuration = _countDownDuration.Subtract(TimeSpan.FromMilliseconds(SLEEP));

                            var oldCount = (int)Math.Round(oldCountDownDuration.TotalSeconds, 0);
                            var newCount = Math.Round(_countDownDuration.TotalSeconds, 0);
                            if (oldCount > newCount)
                            {
                                foreach (var player in Players)
                                    player.Events.Game.SendMatchStartCountdown(oldCount);
                            }

                            if (oldCount <= 0)
                            {
                                foreach (var player in Players)
                                {
                                    /* This disables the countdown timer of the client. */
                                    player.Events.Game.SendMatchStartCountdown(0);

                                    /* 
                                        MatchStart event changes the match state of the client to match running,
                                        which in turn changes the player state to playing.

                                        The client does not care about the roundNumber apparently (in TeamDeatchMatch atleast).
                                     */
                                    player.Events.Game.SendMatchStart(_roundNumber, _endTime);
                                }

                                _countDown = false;
                            }
                        }

                        var position = new List<PlayerMovement>(Players.Count);
                        foreach (var player in Players)
                            position.Add(player.Actor.Movement);

                        var deltas = new List<GameActorInfoDeltaView>(Peers.Count);
                        foreach (var peer in Peers)
                        {
                            var delta = new GameActorInfoDeltaView
                            {
                                Id = peer.Actor.Data.PlayerId
                            };

                            if (DateTime.UtcNow > nextPingUpdate)
                            {
                                delta.Changes.Add(GameActorInfoDeltaView.Keys.Ping, peer.Ping);
                                nextPingUpdate = DateTime.UtcNow.AddSeconds(1);
                            }

                            delta.UpdateMask();
                            deltas.Add(delta);
                        }

                        foreach (var otherPeer in Peers)
                        {
                            otherPeer.Events.Game.SendAllPlayerDeltas(deltas);
                            otherPeer.Events.Game.SendAllPlayerPositions(position, 1);
                        }
                    }
                    catch (Exception ex)
                    {
                        s_log.Error("Failed to tick game loop", ex);
                    }

                    Thread.Sleep(SLEEP);
                }

                s_log.Debug("Game has stopped!");
            }
            catch (ThreadAbortException)
            {
                s_log.Debug("Loop thread was aborted!");
            }
        }
    }
}
