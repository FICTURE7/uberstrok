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

        private bool _waitingForPlayers;

        private readonly Random _rand;

        public TeamDeathMatchGameRoom(GameRoomDataView data) : base(data)
        {
            _waitingForPlayers = true;
            _rand = new Random();

            SpawnPoints = new Dictionary<TeamID, List<SpawnPoint>>();
        }

        public Dictionary<TeamID, List<SpawnPoint>> SpawnPoints { get; set; }
        public List<TimeSpan> PickupRespawnTimes { get; set; }

        protected override void OnJoinTeam(GamePeer peer, TeamID team)
        {
            base.OnJoinTeam(peer, team);

            var point = SpawnPoints[team][_rand.Next(SpawnPoints.Count)];
            if (_waitingForPlayers)
            {
                peer.Events.Game.SendWaitingForPlayer();
                peer.Events.Game.SendPlayerRespawned(peer.Member.CmuneMemberView.PublicProfile.Cmid, point.Position, point.Rotation);
            }
            else
            {
                foreach (var opeer in Peers)
                    opeer.Events.Game.SendPlayerRespawned(peer.Member.CmuneMemberView.PublicProfile.Cmid, point.Position, point.Rotation);
            }

            _waitingForPlayers = Players.Count > 1;

            s_log.Debug($"Spawned: {peer.Member.CmuneMemberView.PublicProfile.Cmid} at: {point}");
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
