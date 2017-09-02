using System;
using System.Collections.Generic;

namespace UbzStuff.Core.Views
{
    [Serializable]
    public class LuckyDrawSetUnityView
    {
        public int CreditsAttributed { get; set; }
        public bool ExposeItemsToPlayers { get; set; }
        public int Id { get; set; }
        public string ImageUrl { get; set; }
        public int LuckyDrawId { get; set; }
        public List<BundleItemView> LuckyDrawSetItems { get; set; }
        public int PointsAttributed { get; set; }
        public int SetWeight { get; set; }
    }
}
