using System;
using UberStrok.Core.Views;

namespace UberStrok.Core
{
    public class LoadoutManager
    {
        public GearManager Gear { get; }
        public WeaponManager Weapons { get; }
        public QuickItemManager QuickItems { get; }

        public LoadoutManager()
        {
            Gear = new GearManager();
            Weapons = new WeaponManager();
            QuickItems = new QuickItemManager();
        }

        public void Update(ShopManager shop, LoadoutView loadout)
        {
            if (shop == null)
                throw new ArgumentNullException(nameof(shop));
            if (loadout == null)
                throw new ArgumentNullException(nameof(loadout));

            Gear.Update(shop, loadout);
            Weapons.Update(shop, loadout);
            QuickItems.Update(shop, loadout);
        }

        public void Reset()
        {
            Weapons.Reset();
            QuickItems.Reset();
        }
    }
}
