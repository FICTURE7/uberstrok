using System.Collections.Generic;
using UberStrok.Core.Common;

namespace UberStrok.Core.Views
{
    public abstract class BaseUberStrikeItemView
    {
        public Dictionary<string, string> CustomProperties { get; set; }
        public string Description { get; set; }
        public int ID { get; set; }
        public bool IsConsumable { get; set; }
        public bool IsForSale => Prices != null && Prices.Count > 0;
        public UberstrikeItemClass ItemClass { get; set; }
        public Dictionary<ItemPropertyType, int> ItemProperties { get; set; }
        public abstract UberstrikeItemType ItemType { get; }
        public int LevelLock { get; set; }
        public int MaxDurationDays { get; set; }
        public string Name { get; set; }
        public string PrefabName { get; set; }
        public ICollection<ItemPriceView> Prices { get; set; }
        public ItemShopHighlightType ShopHighlightType { get; set; }
    }
}
