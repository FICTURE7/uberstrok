using System.IO;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Core.Serialization.Views
{
    public static class LuckyDrawUnityViewProxy
	{
		public static LuckyDrawUnityView Deserialize(Stream bytes)
		{
			var mask = Int32Proxy.Deserialize(bytes);
			var view = new LuckyDrawUnityView();

			view.Category = EnumProxy<BundleCategoryType>.Deserialize(bytes);

			if ((mask & 1) != 0)
				view.Description = StringProxy.Deserialize(bytes);
			if ((mask & 2) != 0)
				view.IconUrl = StringProxy.Deserialize(bytes);

            view.Id = Int32Proxy.Deserialize(bytes);
			view.IsAvailableInShop = BooleanProxy.Deserialize(bytes);

            if ((mask & 4) != 0)
				view.LuckyDrawSets = ListProxy<LuckyDrawSetUnityView>.Deserialize(bytes, LuckyDrawSetUnityViewProxy.Deserialize);
			if ((mask & 8) != 0)
				view.Name = StringProxy.Deserialize(bytes);

            view.Price = Int32Proxy.Deserialize(bytes);
			view.UberStrikeCurrencyType = EnumProxy<UberStrikeCurrencyType>.Deserialize(bytes);
			return view;
		}

		public static void Serialize(Stream stream, LuckyDrawUnityView instance)
		{
			int mask = 0;
			using (var bytes = new MemoryStream())
			{
				EnumProxy<BundleCategoryType>.Serialize(bytes, instance.Category);

                if (instance.Description != null)
					StringProxy.Serialize(bytes, instance.Description);
				else
					mask |= 1;
				if (instance.IconUrl != null)
					StringProxy.Serialize(bytes, instance.IconUrl);
				else
					mask |= 2;

                Int32Proxy.Serialize(bytes, instance.Id);
				BooleanProxy.Serialize(bytes, instance.IsAvailableInShop);
				if (instance.LuckyDrawSets != null)
					ListProxy<LuckyDrawSetUnityView>.Serialize(bytes, instance.LuckyDrawSets, new ListProxy<LuckyDrawSetUnityView>.Serializer<LuckyDrawSetUnityView>(LuckyDrawSetUnityViewProxy.Serialize));
				else
					mask |= 4;
				if (instance.Name != null)
					StringProxy.Serialize(bytes, instance.Name);
				else
					mask |= 8;

                Int32Proxy.Serialize(bytes, instance.Price);
				EnumProxy<UberStrikeCurrencyType>.Serialize(bytes, instance.UberStrikeCurrencyType);
				Int32Proxy.Serialize(stream, ~mask);
				bytes.WriteTo(stream);
			}
		}
	}
}
