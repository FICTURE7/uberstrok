using System;
using System.Collections.Generic;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Realtime.Server.Game
{
    public class TeamDeathMatchGameRoom : BaseGameRoom
    {
        public TeamDeathMatchGameRoom(GameRoomDataView data) : base(data)
        {
            SpawnPoints = new Dictionary<TeamID, List<SpawnPoint>>();
        }

        public Dictionary<TeamID, List<SpawnPoint>> SpawnPoints { get; set; }
        public List<TimeSpan> PickupRespawnTimes { get; set; }

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
