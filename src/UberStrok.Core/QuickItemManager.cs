using System;
using System.Collections;
using System.Collections.Generic;
using UberStrok.Core.Views;

namespace UberStrok.Core
{
    public class QuickItemManager : IEnumerable<QuickItem>
    {
        private readonly QuickItem[] _quickItems;
        private readonly Dictionary<int, QuickItem> _itemId2QuickItem;

        public QuickItem this[int id]
        {
            get
            {
                _itemId2QuickItem.TryGetValue(id, out QuickItem quickItem);
                return quickItem;
            }
        }

        public QuickItemManager()
        {
            _quickItems = new QuickItem[3];
            _itemId2QuickItem = new Dictionary<int, QuickItem>();
        }

        public List<int> GetAsList()
        {
            var list = new List<int>();
            foreach (var quickItem in _quickItems)
                list.Add(quickItem != null ? quickItem.GetView().ID : 0);

            return list;
        }

        public void Update(ShopManager shop, LoadoutView loadout)
        {
            if (shop == null)
                throw new ArgumentNullException(nameof(shop));
            if (loadout == null)
                throw new ArgumentNullException(nameof(loadout));

            QuickItem GetQuickItem(int id)
                => id != 0 ? new QuickItem(shop.QuickItems[id]) : null;

            Array.Clear(_quickItems, 0, _quickItems.Length);

            _quickItems[0] = GetQuickItem(loadout.QuickItem1);
            _quickItems[1] = GetQuickItem(loadout.QuickItem2);
            _quickItems[2] = GetQuickItem(loadout.QuickItem3);

            _itemId2QuickItem.Clear();
            foreach (var quickItem in _quickItems)
            {
                if (quickItem != null)
                    _itemId2QuickItem.Add(quickItem.GetView().ID, quickItem);
            }
        }

        public void Reset()
        {
            foreach (var quickItem in _quickItems)
                quickItem?.Reset();
        }

        public IEnumerator<QuickItem> GetEnumerator()
        {
            foreach (var quickItem in _quickItems)
            {
                if (quickItem != null)
                    yield return quickItem;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
