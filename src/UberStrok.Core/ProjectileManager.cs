using System;
using System.Collections.Generic;

namespace UberStrok.Core
{
    public class ProjectileManager
    {
        private int _numExploded;
        private int _numDestroyed;
        private readonly HashSet<int> _projectiles;

        public int FalsePositive { get; private set; }

        public ProjectileManager()
        {
            _projectiles = new HashSet<int>();
        }

        public void Explode()
        {
            _numExploded++;

            var diff = Math.Abs(_numExploded - _numDestroyed);
            if (diff > 2)
                FalsePositive++;
            else
            {
                _numDestroyed = 0;
                _numExploded = 0;
            }
        }

        public void Emit(int projectileId)
        {
            if (!_projectiles.Add(projectileId))
                FalsePositive++;
        }

        public void Destroy(int projectileId)
        {
            if (!_projectiles.Remove(projectileId))
                FalsePositive++;
            else
                _numDestroyed++;
        }

        public void Reset()
        {
            _projectiles.Clear();
        }
    }
}
