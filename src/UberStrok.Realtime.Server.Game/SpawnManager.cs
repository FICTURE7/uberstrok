using System;
using System.Collections.Generic;
using System.Diagnostics;
using UberStrok.Core.Common;

namespace UberStrok.Realtime.Server.Game
{
    public class SpawnManager
    {
        /* Randomizer we're going to use to spawn stuff. */
        private readonly Random _rand;
        /* List of spawn points of the players in respective teams. */
        private readonly Dictionary<TeamID, List<SpawnPoint>> _spawnPoints;

        public SpawnManager()
        {
            _rand = new Random();
            _spawnPoints = new Dictionary<TeamID, List<SpawnPoint>>();
        }

        public bool IsLoaded(TeamID team) => _spawnPoints.ContainsKey(team);

        public void Load(TeamID team, List<Vector3> positions, List<byte> rotations)
        {
            Debug.Assert(positions.Count == rotations.Count, "Number of spawn positions given and number of rotations given is not equal.");

            int length = positions.Count;
            var spawns = new List<SpawnPoint>(length);
            for (int i = 0; i < length; i++)
                spawns.Add(new SpawnPoint(positions[i], rotations[i]));

            _spawnPoints[team] = spawns;
        }

        public SpawnPoint Get(TeamID team) => _spawnPoints[team][_rand.Next(_spawnPoints.Count)];
    }
}
