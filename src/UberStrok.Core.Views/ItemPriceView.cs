using System;
using UberStrok.Core.Common;

namespace UberStrok.Core.Views
{
    [Serializable]
    public class ItemPriceView
    {
        public int Amount { get; set; }
        public UberStrikeCurrencyType Currency { get; set; }
        public int Discount { get; set; }
        public BuyingDurationType Duration { get; set; }
        public bool IsConsumable => Amount > 0;
        public PackType PackType { get; set; }
        public int Price { get; set; }
    }
}
