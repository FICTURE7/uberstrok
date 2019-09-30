using System;
using System.Collections.Generic;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Core
{
    public class ItemManager
    {
        // Cached ItemShop view to improve performance.
        private readonly UberStrikeItemShopClientView _shopView = new UberStrikeItemShopClientView();
        private readonly Dictionary<int, Item> _items = new Dictionary<int, Item>();

        public ItemManager(UberStrikeItemShopClientView view)
        {
            // TODO: Implement default items.
            // Such as Lutz, name change & clan license.

            if (view == null)
                throw new ArgumentNullException(nameof(view));

            var items = new List<BaseUberStrikeItemView>();
            if (view.WeaponItems != null)
                items.AddRange(view.WeaponItems);
            if (view.GearItems != null)
                items.AddRange(view.GearItems);
            if (view.QuickItems != null)
                items.AddRange(view.QuickItems);
            if (view.FunctionalItems != null)
                items.AddRange(view.FunctionalItems);

            Populate(items);
        }

        public ItemManager(IEnumerable<BaseUberStrikeItemView> items)
        {
            if (items == null)
                throw new ArgumentNullException(nameof(items));

            Populate(items);
        }

        public Item GetItem(int id)
        {
            _items.TryGetValue(id, out Item item);
            return item;
        }

        public Item GetItem(string prefabName)
        {
            var item = default(Item);
            foreach (var v in _items.Values)
            {
                if (v.PrefabName == prefabName)
                {
                    item = v;
                    break;
                }
            }

            return item;
        }

        public Item GetDefaultItem(UberStrikeItemClass itemClass)
        {
            switch (itemClass)
            {
                case UberStrikeItemClass.WeaponMachinegun:
                    break;
            }

            return null;
        }

        public UberStrikeItemShopClientView GetView()
            => _shopView;

        private void Populate(IEnumerable<BaseUberStrikeItemView> itemViews)
        {
            // TODO: Check if prefab names are unique.

            var weaponItems = new List<UberStrikeItemWeaponView>();
            var gearItems = new List<UberStrikeItemGearView>();
            var quickItems = new List<UberStrikeItemQuickView>();
            var functionalItems = new List<UberStrikeItemFunctionalView>();

            _items.Clear();

            foreach (var itemView in itemViews)
            {
                var item = default(Item);

                // NOTE:
                // It is important that these value be not null; otherwise the
                // client will fail to instantiate the prefabs and throw a
                // NullReferenceException.
                if (itemView.CustomProperties == null)
                    itemView.CustomProperties = new Dictionary<string, string>();
                if (itemView.ItemProperties == null)
                    itemView.ItemProperties = new Dictionary<ItemPropertyType, int>();
                if (itemView.Prices == null)
                    itemView.Prices = new List<ItemPriceView>();

                switch (itemView)
                {
                    case UberStrikeItemWeaponView wItem:
                        weaponItems.Add(wItem);
                        item = new Item<UberStrikeItemWeaponView>(wItem);
                        break;
                    case UberStrikeItemGearView gItem:
                        gearItems.Add(gItem);
                        item = new Item<UberStrikeItemGearView>(gItem);
                        break;
                    case UberStrikeItemQuickView qItem:
                        quickItems.Add(qItem);
                        item = new Item<UberStrikeItemQuickView>(qItem);
                        break;
                    case UberStrikeItemFunctionalView fItem:
                        functionalItems.Add(fItem);
                        item = new Item<UberStrikeItemFunctionalView>(fItem);
                        break;

                    default:
                        throw new ArgumentException("Unknown BaseUberStrikeItemView.");
                }

                _items.Add(item.Id, item);
            }

            _shopView.WeaponItems = weaponItems;
            _shopView.GearItems = gearItems;
            _shopView.FunctionalItems = functionalItems;
            _shopView.QuickItems = quickItems;
        }
    }
}
