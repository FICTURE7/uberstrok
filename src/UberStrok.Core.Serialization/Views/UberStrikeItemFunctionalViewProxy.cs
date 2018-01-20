using System.IO;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Core.Serialization.Views
{
    public static class UberStrikeItemFunctionalViewProxy
	{
		public static UberStrikeItemFunctionalView Deserialize(Stream bytes)
		{
			var mask = Int32Proxy.Deserialize(bytes);
			var view = new UberStrikeItemFunctionalView();
			if ((mask & 1) != 0)
				view.CustomProperties = DictionaryProxy<string, string>.Deserialize(bytes, StringProxy.Deserialize, StringProxy.Deserialize);
			if ((mask & 2) != 0)
				view.Description = StringProxy.Deserialize(bytes);

            view.ID = Int32Proxy.Deserialize(bytes);
			view.IsConsumable = BooleanProxy.Deserialize(bytes);
			view.ItemClass = EnumProxy<UberStrikeItemClass>.Deserialize(bytes);
			if ((mask & 4) != 0)
				view.ItemProperties = DictionaryProxy<ItemPropertyType, int>.Deserialize(bytes, EnumProxy<ItemPropertyType>.Deserialize, Int32Proxy.Deserialize);

            view.LevelLock = Int32Proxy.Deserialize(bytes);
			view.MaxDurationDays = Int32Proxy.Deserialize(bytes);

            if ((mask & 8) != 0)
				view.Name = StringProxy.Deserialize(bytes);
			if ((mask & 16) != 0)
				view.PrefabName = StringProxy.Deserialize(bytes);
			if ((mask & 32) != 0)
				view.Prices = ListProxy<ItemPriceView>.Deserialize(bytes, ItemPriceViewProxy.Deserialize);

            view.ShopHighlightType = EnumProxy<ItemShopHighlightType>.Deserialize(bytes);

            return view;
		}

		public static void Serialize(Stream stream, UberStrikeItemFunctionalView instance)
		{
			int mask = 0;
			using (var bytes = new MemoryStream())
			{
				if (instance.CustomProperties != null)
					DictionaryProxy<string, string>.Serialize(bytes, instance.CustomProperties, new DictionaryProxy<string, string>.Serializer<string>(StringProxy.Serialize), new DictionaryProxy<string, string>.Serializer<string>(StringProxy.Serialize));
				else
					mask |= 1;
				if (instance.Description != null)
					StringProxy.Serialize(bytes, instance.Description);
				else
					mask |= 2;

                Int32Proxy.Serialize(bytes, instance.ID);
				BooleanProxy.Serialize(bytes, instance.IsConsumable);
				EnumProxy<UberStrikeItemClass>.Serialize(bytes, instance.ItemClass);

                if (instance.ItemProperties != null)
					DictionaryProxy<ItemPropertyType, int>.Serialize(bytes, instance.ItemProperties, EnumProxy<ItemPropertyType>.Serialize, Int32Proxy.Serialize);
				else
					mask |= 4;

                Int32Proxy.Serialize(bytes, instance.LevelLock);
				Int32Proxy.Serialize(bytes, instance.MaxDurationDays);

                if (instance.Name != null)
					StringProxy.Serialize(bytes, instance.Name);
				else
					mask |= 8;
				if (instance.PrefabName != null)
					StringProxy.Serialize(bytes, instance.PrefabName);
				else
					mask |= 16;
				if (instance.Prices != null)
					ListProxy<ItemPriceView>.Serialize(bytes, instance.Prices, ItemPriceViewProxy.Serialize);
				else
					mask |= 32;

                EnumProxy<ItemShopHighlightType>.Serialize(bytes, instance.ShopHighlightType);
				Int32Proxy.Serialize(stream, ~mask);
				bytes.WriteTo(stream);
			}
		}
	}
}
