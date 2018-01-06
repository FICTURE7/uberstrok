using System.IO;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Core.Serialization.Views
{
    public static class BundleItemViewProxy
	{
		public static BundleItemView Deserialize(Stream bytes)
		{
			return new BundleItemView
			{
				Amount = Int32Proxy.Deserialize(bytes),
				BundleId = Int32Proxy.Deserialize(bytes),
				Duration = EnumProxy<BuyingDurationType>.Deserialize(bytes),
				ItemId = Int32Proxy.Deserialize(bytes)
			};
		}

		public static void Serialize(Stream stream, BundleItemView instance)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Int32Proxy.Serialize(memoryStream, instance.Amount);
				Int32Proxy.Serialize(memoryStream, instance.BundleId);
				EnumProxy<BuyingDurationType>.Serialize(memoryStream, instance.Duration);
				Int32Proxy.Serialize(memoryStream, instance.ItemId);
				memoryStream.WriteTo(stream);
			}
		}
	}
}
