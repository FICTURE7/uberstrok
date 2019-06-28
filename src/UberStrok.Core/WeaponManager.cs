using System;
using System.Collections;
using System.Collections.Generic;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Core
{
    public class WeaponManager : IEnumerable<Weapon>
    {
        private readonly Weapon[] _weapons;
        private readonly Dictionary<int, Weapon> _itemId2Weapon;

        public Weapon this[WeaponSlotType slot] => _weapons[(int)slot];

        public Weapon this[int id]
        {
            get
            {
                _itemId2Weapon.TryGetValue(id, out Weapon weapon);
                return weapon;
            }
        }

        public WeaponManager()
        {
            _weapons = new Weapon[4];
            _itemId2Weapon = new Dictionary<int, Weapon>();
        }

        public List<int> GetAsList()
        {
            var list = new List<int>();
            foreach (var weapon in _weapons)
                list.Add(weapon != null ? weapon.GetView().ID : 0);

            return list;
        }

        public void Update(ShopManager shop, LoadoutView loadout)
        {
            if (shop == null)
                throw new ArgumentNullException(nameof(shop));
            if (loadout == null)
                throw new ArgumentNullException(nameof(loadout));

            Weapon GetWeapon(int id)
                => id != 0 ? new Weapon(shop.WeaponItems[id]) : null;

            Array.Clear(_weapons, 0, _weapons.Length);

            _weapons[0] = GetWeapon(loadout.MeleeWeapon);
            _weapons[1] = GetWeapon(loadout.Weapon1);
            _weapons[2] = GetWeapon(loadout.Weapon2);
            _weapons[3] = GetWeapon(loadout.Weapon3);

            _itemId2Weapon.Clear();
            foreach (var weapon in _weapons)
            {
                if (weapon != null)
                    _itemId2Weapon.Add(weapon.GetView().ID, weapon);
            }
        }

        public void Reset()
        {
            foreach (var weapon in _weapons)
                weapon?.Reset();
        }

        public IEnumerator<Weapon> GetEnumerator()
        {
            foreach (var weapon in _weapons)
            {
                if (weapon != null)
                    yield return weapon;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
