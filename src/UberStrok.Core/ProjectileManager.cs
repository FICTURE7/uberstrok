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

            if (_numExploded - _numEmitted > 20)
            {
                FalsePositive++;

                _numExploded = 0;
                _numEmitted = 0;
            }
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
