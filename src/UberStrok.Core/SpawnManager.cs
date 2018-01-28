using System;
using System.Collections.Generic;
using UberStrok.Core.Common;

namespace UberStrok.Core
{
    public class SpawnManager
    {
        private int _index;
        private int _spawnCount;

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
            if (positions == null)
                throw new ArgumentNullException(nameof(positions));
            if (rotations == null)
                throw new ArgumentNullException(nameof(rotations));
            
            int length = positions.Count;
            var spawns = new List<SpawnPoint>(length);
            for (int i = 0; i < length; i++)
                spawns.Add(new SpawnPoint(positions[i], rotations[i]));

            _spawnPoints[team] = spawns;
            _index = _rand.Next(_spawnPoints.Count);
        }

        public SpawnPoint Get(TeamID team)
        {
            /* Slightly less random spawns. */
            if (_spawnCount % 5 == 0)
                _index = _rand.Next(_spawnPoints.Count);
            else
                _index++;

            return _spawnPoints[team][_index];
        }
    }
}
