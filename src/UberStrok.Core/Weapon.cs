using System;
using UberStrok.Core.Views;

namespace UberStrok.Core
{
    public class Weapon
    {
        public int FalsePositiveThreshold { get; }
        public int FalsePositive { get; private set; }
        public UberStrikeItemWeaponView View { get; }

        public bool CanTrigger => (DateTime.UtcNow - _lastShot).TotalMilliseconds >= View.RateOfFire;

        private DateTime _lastShot;

        public Weapon(UberStrikeItemWeaponView view)
        {
            View = view ?? throw new ArgumentNullException(nameof(view));

            FalsePositiveThreshold = CalculateThreshold(View.RateOfFire);
            _lastShot = DateTime.UtcNow;
        }

        public void Trigger()
        {
            if (CanTrigger)
            {
                _lastShot = DateTime.UtcNow;
            }
            else
                FalsePositive++;
        }

        public void Reset()
        {
            _lastShot = DateTime.UtcNow;
            FalsePositive = 0;
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
