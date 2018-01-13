﻿using log4net;
using System;
using System.Collections.Generic;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Game
{
    public class TeamDeathMatchGameRoom : BaseGameRoom
    {
        private readonly static ILog s_log = LogManager.GetLogger(nameof(TeamDeathMatchGameRoom));

        private bool _started;
        private int _endTime;

        private readonly Dictionary<TeamID, List<SpawnPoint>> _spawnPoints;
        private readonly Random _rand;

        public TeamDeathMatchGameRoom(GameRoomDataView data) : base(data)
        {
            _started = false;
            _rand = new Random();
            _spawnPoints = new Dictionary<TeamID, List<SpawnPoint>>();
        }

        public Dictionary<TeamID, List<SpawnPoint>> SpawnPoints => _spawnPoints;
        public List<TimeSpan> PickupRespawnTimes { get; set; }

        public override void Join(GamePeer peer)
        {
            base.Join(peer);

            var allPlayers = new List<GameActorInfoView>(Players.Count);
            var allPositions = new List<PlayerMovement>(Players.Count);
            foreach (var playerPeer in Players)
            {
                allPlayers.Add(playerPeer.Actor.View);
                /* Actor.Movement can be null when the actor is in the 'waiting for players' state. */
                allPositions.Add(playerPeer.Actor.Movement ?? new PlayerMovement());
            }

            s_log.Debug($"Sent {Players.Count}");
            peer.Events.Game.SendAllPlayers(allPlayers, allPositions, 0);
        }

        public override void Leave(GamePeer peer)
        {
            base.Leave(peer);
            foreach (var otherPeer in Peers)
            {
                if (otherPeer.Actor.Cmid != peer.Actor.Cmid)
                    otherPeer.Events.Game.SendPlayerLeftGame(peer.Actor.Cmid);
            }

            peer.Actor = null;
        }

        private void StartMatch()
        {
            _endTime = Environment.TickCount + Data.TimeLimit * 1000;
            foreach (var peer in Peers)
            {
                var point = GetRandomSpawn(peer);
                var movement = new PlayerMovement
                {
                    Position = point.Position,
                    HorizontalRotation = point.Rotation
                };

                peer.Actor.Movement = movement;

                peer.Events.Game.SendMatchStart(0, _endTime);
                peer.Events.Game.SendPlayerJoinedGame(peer.Actor.View, movement);
                peer.Events.Game.SendPlayerRespawned(peer.Member.CmuneMemberView.PublicProfile.Cmid, point.Position, point.Rotation);

                s_log.Debug($"Spawned: {peer.Member.CmuneMemberView.PublicProfile.Cmid} at: {point}");
            }

            _started = true;
        }

        private SpawnPoint GetRandomSpawn(GamePeer peer)
        {
            var point = SpawnPoints[peer.Actor.Team][_rand.Next(SpawnPoints.Count)];
            return point;
        }

        protected override void OnJoinTeam(GamePeer peer, TeamID team)
        {
            base.OnJoinTeam(peer, team);

            if (!_started && Players.Count > 1)
            {
                StartMatch();
                return;
            }

            if (!_started)
            {
                peer.Events.Game.SendPlayerJoinedGame(peer.Actor.View, new PlayerMovement());
                peer.Events.Game.SendWaitingForPlayer();
            }
            else
            {
                var point = GetRandomSpawn(peer);
                var movement = new PlayerMovement
                {
                    Position = point.Position,
                    HorizontalRotation = point.Rotation
                };

                peer.Actor.Movement = movement;

                foreach (var otherPeer in Peers)
                    otherPeer.Events.Game.SendPlayerJoinedGame(peer.Actor.View, movement);

                peer.Events.Game.SendMatchStart(0, _endTime);
                peer.Events.Game.SendPlayerRespawned(peer.Member.CmuneMemberView.PublicProfile.Cmid, point.Position, point.Rotation);
            }
        }

        protected override void OnSpawnPositions(GamePeer peer, TeamID team, List<Vector3> positions, List<byte> rotations)
        {
            base.OnSpawnPositions(peer, team, positions, rotations);

            /* We care only about the first operation sent for that team ID. */
            if (SpawnPoints.ContainsKey(team))
                return;

            int length = positions.Count;
            var spawns = new List<SpawnPoint>(length);
            for (int i = 0; i < length; i++)
            {
                var point = new SpawnPoint(positions[i], rotations[i]);
                spawns.Add(point);
            }

            SpawnPoints.Add(team, spawns);
        }

        protected override void OnPowerUpRespawnTimes(GamePeer peer, List<ushort> respawnTimes)
        {
            base.OnPowerUpRespawnTimes(peer, respawnTimes);

            /* We care only about the first operation sent. */
            if (PickupRespawnTimes != null)
                return;

            var length = respawnTimes.Count;
            PickupRespawnTimes = new List<TimeSpan>(length);

            for (int i = 0; i < length; i++)
            {
                var time = TimeSpan.FromSeconds(respawnTimes[i]);
                PickupRespawnTimes.Add(time);
            }
        }

        protected override void OnIsFiring(GamePeer peer, bool on)
        {
            // Space
        }

        protected override void OnJump(GamePeer peer, Vector3 position)
        {
            // Space
        }

        protected override void OnUpdatePositionAndRotation(GamePeer peer, Vector3 position, Vector3 velocity, byte horizontalRotation, byte verticalRotation, byte moveState)
        {
            // Space
        }

        protected override void OnSwitchWeapon(GamePeer peer, byte slot)
        {
            // Space
        }

        protected override void OnIsPaued(GamePeer peer, bool on)
        {
            // Space
        }
    }
}
