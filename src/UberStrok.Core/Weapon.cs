using System;
using UberStrok.Core.Views;

namespace UberStrok.Core
{
    public class Weapon
    {
        public int FalsePositive { get; private set; }
        public UberStrikeItemWeaponView View { get; }

        private DateTime _lastShot;

        public Weapon(UberStrikeItemWeaponView view)
        {
            View = view ?? throw new ArgumentNullException(nameof(view));

            _lastShot = DateTime.UtcNow;
        }

        public bool CanTrigger()
        {
            return (DateTime.UtcNow - _lastShot).TotalMilliseconds >= View.RateOfFire;
        }

        public void Trigger()
        {
            if (CanTrigger())
                _lastShot = DateTime.UtcNow;
            else
                FalsePositive++;
        }

        public void Reset()
        {
            _lastShot = DateTime.UtcNow;
        }
    }
}
