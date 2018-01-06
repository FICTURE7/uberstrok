using System;
using UberStrok.Core.Common;

namespace UberStrok.Core.Views
{
	[Serializable]
	public class BundleItemView
	{
		public int Amount { get; set; }
		public int BundleId { get; set; }
		public BuyingDurationType Duration { get; set; }
		public int ItemId { get; set; }
	}
}
