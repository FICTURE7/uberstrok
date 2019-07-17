using System;
using System.Collections.Generic;
using UberStrok.Core.Common;

namespace UberStrok.Core.Views
{
    [Serializable]
    public class BundleView
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string IconUrl { get; set; }
        public string Description { get; set; }
        public bool IsOnSale { get; set; }
        public bool IsPromoted { get; set; }
        public decimal USDPrice { get; set; }
        public decimal USDPromoPrice { get; set; }
        public int Credits { get; set; }
        public int Points { get; set; }
        public List<BundleItemView> BundleItemViews { get; set; }
        public BundleCategoryType Category { get; set; }
        public List<ChannelType> Availability { get; set; }
        public string PromotionTag { get; set; }
        public string MacAppStoreUniqueId { get; set; }
        public string IosAppStoreUniqueId { get; set; }
        public string AndroidStoreUniqueId { get; set; }
        public bool IsDefault { get; set; }
    }
}