using System.IO;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Core.Serialization.Views
{
    public static class LoadoutViewProxy
	{
		public static LoadoutView Deserialize(Stream bytes)
		{
			int mask = Int32Proxy.Deserialize(bytes);
			var view = new LoadoutView();
			view.Backpack = Int32Proxy.Deserialize(bytes);
			view.Boots = Int32Proxy.Deserialize(bytes);
			view.Cmid = Int32Proxy.Deserialize(bytes);
			view.Face = Int32Proxy.Deserialize(bytes);
			view.FunctionalItem1 = Int32Proxy.Deserialize(bytes);
			view.FunctionalItem2 = Int32Proxy.Deserialize(bytes);
			view.FunctionalItem3 = Int32Proxy.Deserialize(bytes);
			view.Gloves = Int32Proxy.Deserialize(bytes);
			view.Head = Int32Proxy.Deserialize(bytes);
			view.LoadoutId = Int32Proxy.Deserialize(bytes);
			view.LowerBody = Int32Proxy.Deserialize(bytes);
			view.MeleeWeapon = Int32Proxy.Deserialize(bytes);
			view.QuickItem1 = Int32Proxy.Deserialize(bytes);
			view.QuickItem2 = Int32Proxy.Deserialize(bytes);
			view.QuickItem3 = Int32Proxy.Deserialize(bytes);

			if ((mask & 1) != 0)
				view.SkinColor = StringProxy.Deserialize(bytes);

			view.Type = EnumProxy<AvatarType>.Deserialize(bytes);
			view.UpperBody = Int32Proxy.Deserialize(bytes);
			view.Weapon1 = Int32Proxy.Deserialize(bytes);
			view.Weapon1Mod1 = Int32Proxy.Deserialize(bytes);
			view.Weapon1Mod2 = Int32Proxy.Deserialize(bytes);
			view.Weapon1Mod3 = Int32Proxy.Deserialize(bytes);
			view.Weapon2 = Int32Proxy.Deserialize(bytes);
			view.Weapon2Mod1 = Int32Proxy.Deserialize(bytes);
			view.Weapon2Mod2 = Int32Proxy.Deserialize(bytes);
			view.Weapon2Mod3 = Int32Proxy.Deserialize(bytes);
			view.Weapon3 = Int32Proxy.Deserialize(bytes);
			view.Weapon3Mod1 = Int32Proxy.Deserialize(bytes);
			view.Weapon3Mod2 = Int32Proxy.Deserialize(bytes);
			view.Weapon3Mod3 = Int32Proxy.Deserialize(bytes);
			view.Webbing = Int32Proxy.Deserialize(bytes);

			return view;
		}

		public static void Serialize(Stream stream, LoadoutView instance)
		{
			int mask = 0;
			using (var bytes = new MemoryStream())
			{
				Int32Proxy.Serialize(bytes, instance.Backpack);
				Int32Proxy.Serialize(bytes, instance.Boots);
				Int32Proxy.Serialize(bytes, instance.Cmid);
				Int32Proxy.Serialize(bytes, instance.Face);
				Int32Proxy.Serialize(bytes, instance.FunctionalItem1);
				Int32Proxy.Serialize(bytes, instance.FunctionalItem2);
				Int32Proxy.Serialize(bytes, instance.FunctionalItem3);
				Int32Proxy.Serialize(bytes, instance.Gloves);
				Int32Proxy.Serialize(bytes, instance.Head);
				Int32Proxy.Serialize(bytes, instance.LoadoutId);
				Int32Proxy.Serialize(bytes, instance.LowerBody);
				Int32Proxy.Serialize(bytes, instance.MeleeWeapon);
				Int32Proxy.Serialize(bytes, instance.QuickItem1);
				Int32Proxy.Serialize(bytes, instance.QuickItem2);
				Int32Proxy.Serialize(bytes, instance.QuickItem3);

				if (instance.SkinColor != null)
					StringProxy.Serialize(bytes, instance.SkinColor);
				else
					mask |= 1;

				EnumProxy<AvatarType>.Serialize(bytes, instance.Type);
				Int32Proxy.Serialize(bytes, instance.UpperBody);
				Int32Proxy.Serialize(bytes, instance.Weapon1);
				Int32Proxy.Serialize(bytes, instance.Weapon1Mod1);
				Int32Proxy.Serialize(bytes, instance.Weapon1Mod2);
				Int32Proxy.Serialize(bytes, instance.Weapon1Mod3);
				Int32Proxy.Serialize(bytes, instance.Weapon2);
				Int32Proxy.Serialize(bytes, instance.Weapon2Mod1);
				Int32Proxy.Serialize(bytes, instance.Weapon2Mod2);
				Int32Proxy.Serialize(bytes, instance.Weapon2Mod3);
				Int32Proxy.Serialize(bytes, instance.Weapon3);
				Int32Proxy.Serialize(bytes, instance.Weapon3Mod1);
				Int32Proxy.Serialize(bytes, instance.Weapon3Mod2);
				Int32Proxy.Serialize(bytes, instance.Weapon3Mod3);
				Int32Proxy.Serialize(bytes, instance.Webbing);
				Int32Proxy.Serialize(stream, ~mask);
				bytes.WriteTo(stream);
			}
		}
	}
}
