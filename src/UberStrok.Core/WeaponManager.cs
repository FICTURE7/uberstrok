using System.Collections.Generic;
using UberStrok.Core.Views;

namespace UberStrok.Core
{
    public class WeaponManager
    {
        private Dictionary<int, Weapon> _weapons;

        public Weapon this[int id]
        {
            get
            {
                _weapons.TryGetValue(id, out Weapon weapon);
                return weapon;
            }
        }

        public WeaponManager()
        {
            _weapons = new Dictionary<int, Weapon>();
        }

        public void Update(List<UberStrikeItemWeaponView> items)
        {
            _weapons.Clear();
            foreach (var item in items)
                _weapons.Add(item.ID, new Weapon(item));
        }

        public void Reset()
        {
            foreach (var weapon in _weapons.Values)
                weapon.Reset();
        }
    }
}
