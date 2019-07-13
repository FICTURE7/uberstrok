using System;
using UberStrok.Core.Views;

namespace UberStrok.Core
{
    public class Weapon
    {
        private bool _firing;
        private DateTime _fireTime;

        private readonly UberStrikeItemWeaponView _view;

        public int FalsePositiveThreshold { get; }
        public int FalsePositive { get; private set; }

        public bool CanHit => (DateTime.UtcNow - _lastHit).TotalMilliseconds >= _view.RateOfFire;

        private DateTime _lastHit;

        public Weapon(UberStrikeItemWeaponView view)
        {
            _view = view ?? throw new ArgumentNullException(nameof(view));

            FalsePositiveThreshold = CalculateThreshold(_view.RateOfFire);
            _lastHit = DateTime.UtcNow;
        }

        public void StartFire()
        {
            if (!_firing)
            {
                _fireTime = DateTime.UtcNow;
                _firing = true;
            }
        }

        public int StopFire()
        {
            if (!_firing)
                return 0;

            _firing = false;
            return (int)Math.Ceiling((DateTime.UtcNow - _fireTime).TotalMilliseconds / _view.RateOfFire);
        }

        public void Hit()
        {
            if (CanHit)
                _lastHit = DateTime.UtcNow;
            else
                FalsePositive++;
        }

        public void Reset()
        {
            _firing = false;
            _fireTime = DateTime.UtcNow;
            _lastHit = DateTime.UtcNow;

            FalsePositive = 0;
        }

        public UberStrikeItemWeaponView GetView()
        {
            return _view;
        }

        private static int CalculateThreshold(int rof)
        {
            int threshold = (int)Math.Round(-0.016f * rof + 20.8f);
            if (threshold < 5)
                threshold = 5;
            return threshold;
        }
    }
}
