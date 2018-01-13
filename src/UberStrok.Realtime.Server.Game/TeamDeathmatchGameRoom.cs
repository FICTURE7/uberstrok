using log4net;
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

        private void StartMatch()
        {
            foreach (var peer in Peers)
            {
                var point = GetRandomSpawn(peer);

                peer.Events.Game.SendMatchStart(0, Data.TimeLimit * 1000);
                peer.Events.Game.SendPlayerJoinGame(peer.Actor, new PlayerMovement());
                peer.Events.Game.SendPlayerRespawned(peer.Member.CmuneMemberView.PublicProfile.Cmid, point.Position, point.Rotation);

                s_log.Debug($"Spawned: {peer.Member.CmuneMemberView.PublicProfile.Cmid} at: {point}");
            }

            _started = true;
        }

        private SpawnPoint GetRandomSpawn(GamePeer peer)
        {
            var point = SpawnPoints[peer.Actor.TeamID][_rand.Next(SpawnPoints.Count)];
            return point;
        }

        protected override void OnJoinTeam(GamePeer peer, TeamID team)
        {
            base.OnJoinTeam(peer, team);

            var point = GetRandomSpawn(peer);
            if (!_started)
                peer.Events.Game.SendWaitingForPlayer();
            else
                peer.Events.Game.SendMatchStart(0, 0);

            peer.Events.Game.SendPlayerRespawned(peer.Member.CmuneMemberView.PublicProfile.Cmid, point.Position, point.Rotation);

            if (!_started && Players.Count > 1)
                StartMatch();
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
    }
}
