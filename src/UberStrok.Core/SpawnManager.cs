using System;
using System.Collections.Generic;
using UberStrok.Core.Common;

namespace UberStrok.Core
{
    public class SpawnManager
    {
        /* Randomizer we're going to use to spawn stuff. */
        private readonly Random _rand;
        /* List of spawn points of the players in respective teams. */
        private readonly Dictionary<TeamID, List<SpawnPoint>> _points;
        private readonly Dictionary<TeamID, int> _indices;

        public SpawnManager()
        {
            _rand = new Random();
            _indices = new Dictionary<TeamID, int>();
            _points = new Dictionary<TeamID, List<SpawnPoint>>();
        }

        public bool IsLoaded(TeamID team) => _points.ContainsKey(team);

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

            _points[team] = spawns;
            _indices[team] = _rand.Next(_points.Count);
        }

        public SpawnPoint Get(TeamID team)
        {
            /* Incase stuff goes loose. */
            if (!_indices.ContainsKey(team) || !_points.ContainsKey(team))
                return default(SpawnPoint);

            var point = _points[team][_indices[team]++];
            var index = _indices[team];

            if (index % _points[team].Count == 0)
                _indices[team] = _rand.Next(_points.Count);

            return point;
        }
    }
}
