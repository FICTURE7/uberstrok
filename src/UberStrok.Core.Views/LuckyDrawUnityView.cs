using System;
using System.Collections.Generic;
using UberStrok.Core.Common;

namespace UberStrok.Core.Views
{
	[Serializable]
	public class LuckyDrawUnityView
	{
		public BundleCategoryType Category { get; set; }
		public string Description { get; set; }
		public string IconUrl { get; set; }
		public int Id { get; set; }
		public bool IsAvailableInShop { get; set; }
		public List<LuckyDrawSetUnityView> LuckyDrawSets { get; set; }
		public string Name { get; set; }
		public int Price { get; set; }
		public UberStrikeCurrencyType UberStrikeCurrencyType { get; set; }
	}
}
