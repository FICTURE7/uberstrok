using System.IO;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Core.Serialization.Views
{
    public static class ItemPriceViewProxy
	{
		public static ItemPriceView Deserialize(Stream bytes)
		{
			return new ItemPriceView
			{
				Amount = Int32Proxy.Deserialize(bytes),
				Currency = EnumProxy<UberStrikeCurrencyType>.Deserialize(bytes),
				Discount = Int32Proxy.Deserialize(bytes),
				Duration = EnumProxy<BuyingDurationType>.Deserialize(bytes),
				PackType = EnumProxy<PackType>.Deserialize(bytes),
				Price = Int32Proxy.Deserialize(bytes)
			};
		}

		public static void Serialize(Stream stream, ItemPriceView instance)
		{
			using (var bytes = new MemoryStream())
			{
				Int32Proxy.Serialize(bytes, instance.Amount);
				EnumProxy<UberStrikeCurrencyType>.Serialize(bytes, instance.Currency);
				Int32Proxy.Serialize(bytes, instance.Discount);
				EnumProxy<BuyingDurationType>.Serialize(bytes, instance.Duration);
				EnumProxy<PackType>.Serialize(bytes, instance.PackType);
				Int32Proxy.Serialize(bytes, instance.Price);
				bytes.WriteTo(stream);
			}
		}
	}
}
