using System;
using System.Collections;
using System.Collections.Generic;
using UberStrok.Core.Views;

namespace UberStrok.Core
{
    public class GearManager : IEnumerable<Gear>
    {
        private readonly Gear[] _gear;
        private readonly Dictionary<int, Gear> _itemId2Gear;

        public Gear this[int id]
        {
            get
            {
                _itemId2Gear.TryGetValue(id, out Gear gear);
                return gear;
            }
        }

        public GearManager()
        {
            _gear = new Gear[7];
            _itemId2Gear = new Dictionary<int, Gear>();
        }

        public List<int> GetAsList()
        {
            var list = new List<int>();
            foreach (var gear in _gear)
                list.Add(gear != null ? gear.GetView().ID : 0);

            return list;
        }

        public void Update(ShopManager shop, LoadoutView loadout)
        {
            if (shop == null)
                throw new ArgumentNullException(nameof(shop));
            if (loadout == null)
                throw new ArgumentNullException(nameof(loadout));

            Gear GetGear(int id)
                => id != 0 ? new Gear(shop.GearItems[id]) : null;

            Array.Clear(_gear, 0, _gear.Length);

            _gear[0] = GetGear(loadout.Webbing);
            _gear[1] = GetGear(loadout.Head);
            _gear[2] = GetGear(loadout.Face);
            _gear[3] = GetGear(loadout.Gloves);
            _gear[4] = GetGear(loadout.UpperBody);
            _gear[5] = GetGear(loadout.LowerBody);
            _gear[6] = GetGear(loadout.Boots);

            _itemId2Gear.Clear();
            foreach (var gear in _gear)
            {
                if (gear != null)
                    _itemId2Gear.Add(gear.GetView().ID, gear);
            }
        }

        public byte GetArmorCapacity()
        {
            int armorCapacity = 0;
            foreach (var gear in _gear)
            {
                if (gear != null)
                    armorCapacity += gear.GetView().ArmorPoints;
            }

            /* Clamp armor capacity to 200. */
            return (byte)Math.Min(200, armorCapacity);
        }

        public IEnumerator<Gear> GetEnumerator()
        {
            foreach (var gear in _gear)
            {
                if (gear != null)
                    yield return gear;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
