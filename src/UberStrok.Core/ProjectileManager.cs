using System;
using System.Collections.Generic;

namespace UberStrok.Core
{
    public class ProjectileManager
    {
        private int _numExploded;
        private int _numEmitted;
        private readonly HashSet<int> _projectiles;

        public int FalsePositive { get; private set; }

        public ProjectileManager()
        {
            _projectiles = new HashSet<int>();
        }

        public void Explode()
        {
            _numExploded++;

            var diff = Math.Abs(_numEmitted - _numExploded);
            if (diff > 50)
                FalsePositive++;
        }

        public void Emit(int projectileId)
        {
            _projectiles.Add(projectileId);
            _numEmitted++;
        }

        public void Destroy(int projectileId)
        {
            _projectiles.Remove(projectileId);
        }

        public void Reset()
        {
            _projectiles.Clear();
        }
    }
}
