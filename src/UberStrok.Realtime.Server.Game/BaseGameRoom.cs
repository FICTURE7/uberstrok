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

        /* Determine wether the game has started. */
        private bool _started;
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
        private readonly IReadOnlyCollection<GamePeer> _peersReadOnly;
        private readonly IReadOnlyCollection<GamePeer> _playersReadonly;

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
        public IReadOnlyCollection<GamePeer> Peers => _peersReadOnly;
        public IReadOnlyCollection<GamePeer> Players => _playersReadonly;

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

            var actor = new GameActor(data);

            lock (_peers)
            {
                _peers.Add(peer);

                data.PlayerId = (byte)_peers.Count;
                actor.Movement.Number = data.PlayerId;
            }

            peer.Room = this;
            peer.Actor = actor;
            peer.AddOperationHandler(this);

            /* 
                This prepares the client for the game room and
                sets the client state to pre-game.
             */
            peer.Events.SendRoomEntered(Data);

            /* Let the client know about the other peers in the room, if there is any.*/
            if (Players.Count > 1)
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
            {
                if (otherPeer.Actor.Cmid != peer.Actor.Cmid)
                    otherPeer.Events.Game.SendPlayerLeftGame(peer.Actor.Cmid);
            }

            /* If we have 0 players, we stop the game so we can start a new one later on. */
            if (Players.Count == 0)
                _started = false;

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
            /* Kill the previous loop thread. */
            if (_loopThread != null)
                _loopThread.Abort();

            _started = true;

            _loopThread = new Thread(GameLoop);
            _loopThread.Start();

            /* Calculate the time when the games ends (in system ticks). */
            _endTime = Environment.TickCount + Data.TimeLimit * 1000;

            s_log.Debug($"Started new match: {_roundNumber} which should end at system tick: {_endTime}.");

            foreach (var player in Players)
            {
                var point = _spawnManager.Get(player.Actor.Team);
                var movement = player.Actor.Movement;
                movement.Number = player.Actor.Data.PlayerId;
                movement.Position = point.Position;
                movement.HorizontalRotation = point.Rotation;

                //player.Actor.Movement = movement;

                /* Let all peers know that the peer has joined the game. */
                foreach (var otherPeer in Peers)
                {
                    /*
                        PlayerJoinedGame event tells the client to initiate the character and register it
                        in its player list and update the team player number counts.
                     */
                    otherPeer.Events.Game.SendPlayerJoinedGame(player.Actor.Data, movement);
                    otherPeer.Events.Game.SendPlayerRespawned(player.Actor.Cmid, movement.Position, movement.HorizontalRotation);
                }

                /* 
                    MatchStart event changes the match state of the client to match running,
                    which in turn changes the player state to playing.

                    The client does not care about the roundNumber apparently (in TeamDeatchMatch atleast).
                 */
                player.Events.Game.SendMatchStart(_roundNumber, _endTime);
                player.Events.Game.SendPlayerRespawned(player.Actor.Cmid, point.Position, point.Rotation);

                s_log.Debug($"Spawned: {player.Actor.Cmid} at: {point}");
            }

            _roundNumber++;
        }

        protected override void OnJoinGame(GamePeer peer, TeamID team)
        {
            /* 
                Update the actor's team and register the peer in the player list.
                Update the number of connected players while we're at it.
             */
            peer.Actor.Team = team;
            peer.Actor.Data.Health = 100;

            lock (_peers)
            {
                _players.Add(peer);
                _data.ConnectedPlayers = Players.Count;
            }

            s_log.Info($"Joining team -> CMID:{peer.Actor.Cmid}:{team}");

            /*
                If the match has not started yet and there is more
                than 1 players, we start the match.
             */
            if (!_started && Players.Count > 1)
            {
                StartMatch();
                return;
            }

            var movement = default(PlayerMovement);
            if (!_started)
            {
                movement = new PlayerMovement
                {
                    Number = peer.Actor.Data.PlayerId
                };

                /* Let all peers know that the client has joined. */
                foreach (var otherPeer in Peers)
                    otherPeer.Events.Game.SendPlayerJoinedGame(peer.Actor.Data, movement);

                peer.Events.Game.SendWaitingForPlayer();
            }
            else
            {
                var point = _spawnManager.Get(peer.Actor.Team);
                movement = new PlayerMovement
                {
                    Number = peer.Actor.Data.PlayerId,
                    Position = point.Position,
                    HorizontalRotation = point.Rotation
                };

                /* Let all peers know that the client has joined. */
                foreach (var otherPeer in Peers)
                    otherPeer.Events.Game.SendPlayerJoinedGame(peer.Actor.Data, movement);

                peer.Events.Game.SendMatchStart(0, _endTime);
                peer.Events.Game.SendPlayerRespawned(peer.Actor.Cmid, point.Position, point.Rotation);
            }

            //peer.Actor.Movement = movement;
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

            const int TICK_RATE = 64;
            const int SLEEP = 1000 / TICK_RATE;

            //TODO: Make the loop fancier using catch-up stuffz & tings.
            //TODO: Fix potential threading issues.
            try
            {
                while (_started)
                {
                    try
                    {
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
                            otherPeer.Events.Game.SendAllPlayerPositions(position, 100);
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
