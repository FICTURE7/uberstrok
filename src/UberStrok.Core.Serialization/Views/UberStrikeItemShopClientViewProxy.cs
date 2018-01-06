using System.IO;
using UberStrok.Core.Views;

namespace UberStrok.Core.Serialization.Views
{
    public static class UberStrikeItemShopClientViewProxy
	{
		public static UberStrikeItemShopClientView Deserialize(Stream bytes)
		{
			var mask = Int32Proxy.Deserialize(bytes);
			var view = new UberStrikeItemShopClientView();
            if ((mask & 1) != 0)
                view.FunctionalItems = ListProxy<UberStrikeItemFunctionalView>.Deserialize(bytes, UberStrikeItemFunctionalViewProxy.Deserialize);
			if ((mask & 2) != 0)
				view.GearItems = ListProxy<UberStrikeItemGearView>.Deserialize(bytes, UberStrikeItemGearViewProxy.Deserialize);
            if ((mask & 4) != 0)
                view.QuickItems = ListProxy<UberStrikeItemQuickView>.Deserialize(bytes, UberStrikeItemQuickViewProxy.Deserialize);
			if ((mask & 8) != 0)
				view.WeaponItems = ListProxy<UberStrikeItemWeaponView>.Deserialize(bytes, UberStrikeItemWeaponViewProxy.Deserialize);

			return view;
		}

		public static void Serialize(Stream stream, UberStrikeItemShopClientView instance)
		{
			int mask = 0;
			using (var bytes = new MemoryStream())
			{
				if (instance.FunctionalItems != null)
					ListProxy<UberStrikeItemFunctionalView>.Serialize(bytes, instance.FunctionalItems, UberStrikeItemFunctionalViewProxy.Serialize);
				else
					mask |= 1;
				if (instance.GearItems != null)
					ListProxy<UberStrikeItemGearView>.Serialize(bytes, instance.GearItems, UberStrikeItemGearViewProxy.Serialize);
				else
					mask |= 2;
				if (instance.QuickItems != null)
					ListProxy<UberStrikeItemQuickView>.Serialize(bytes, instance.QuickItems,UberStrikeItemQuickViewProxy.Serialize);
				else
					mask |= 4;
				if (instance.WeaponItems != null)
					ListProxy<UberStrikeItemWeaponView>.Serialize(bytes, instance.WeaponItems, UberStrikeItemWeaponViewProxy.Serialize);
				else
					mask |= 8;

                Int32Proxy.Serialize(stream, ~mask);
				bytes.WriteTo(stream);
			}
		}
	}
}
