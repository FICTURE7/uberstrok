using System.IO;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Core.Serialization.Views
{
    public static class UberStrikeItemWeaponViewProxy
	{
		public static UberStrikeItemWeaponView Deserialize(Stream bytes)
		{
			int mask = Int32Proxy.Deserialize(bytes);
			var view = new UberStrikeItemWeaponView();
			view.AccuracySpread = Int32Proxy.Deserialize(bytes);
			view.CombatRange = Int32Proxy.Deserialize(bytes);
			view.CriticalStrikeBonus = Int32Proxy.Deserialize(bytes);

			if ((mask & 1) != 0)
				view.CustomProperties = DictionaryProxy<string, string>.Deserialize(bytes, StringProxy.Deserialize, StringProxy.Deserialize);

			view.DamageKnockback = Int32Proxy.Deserialize(bytes);
			view.DamagePerProjectile = Int32Proxy.Deserialize(bytes);
			view.DefaultZoomMultiplier = Int32Proxy.Deserialize(bytes);

			if ((mask & 2) != 0)
				view.Description = StringProxy.Deserialize(bytes);

			view.HasAutomaticFire = BooleanProxy.Deserialize(bytes);
			view.ID = Int32Proxy.Deserialize(bytes);
			view.IsConsumable = BooleanProxy.Deserialize(bytes);
			view.ItemClass = EnumProxy<UberStrikeItemClass>.Deserialize(bytes);

			if ((mask & 4) != 0)
				view.ItemProperties = DictionaryProxy<ItemPropertyType, int>.Deserialize(bytes, EnumProxy<ItemPropertyType>.Deserialize, Int32Proxy.Deserialize);

			view.LevelLock = Int32Proxy.Deserialize(bytes);
			view.MaxAmmo = Int32Proxy.Deserialize(bytes);
			view.MaxDurationDays = Int32Proxy.Deserialize(bytes);
			view.MaxZoomMultiplier = Int32Proxy.Deserialize(bytes);
			view.MinZoomMultiplier = Int32Proxy.Deserialize(bytes);
			view.MissileBounciness = Int32Proxy.Deserialize(bytes);
			view.MissileForceImpulse = Int32Proxy.Deserialize(bytes);
			view.MissileTimeToDetonate = Int32Proxy.Deserialize(bytes);

			if ((mask & 8) != 0)
				view.Name = StringProxy.Deserialize(bytes);
			if ((mask & 16) != 0)
				view.PrefabName = StringProxy.Deserialize(bytes);
			if ((mask & 32) != 0)
				view.Prices = ListProxy<ItemPriceView>.Deserialize(bytes, ItemPriceViewProxy.Deserialize);

			view.ProjectileSpeed = Int32Proxy.Deserialize(bytes);
			view.ProjectilesPerShot = Int32Proxy.Deserialize(bytes);
			view.RateOfFire = Int32Proxy.Deserialize(bytes);
			view.RecoilKickback = Int32Proxy.Deserialize(bytes);
			view.RecoilMovement = Int32Proxy.Deserialize(bytes);
			view.SecondaryActionReticle = Int32Proxy.Deserialize(bytes);
			view.ShopHighlightType = EnumProxy<ItemShopHighlightType>.Deserialize(bytes);
			view.SplashRadius = Int32Proxy.Deserialize(bytes);
			view.StartAmmo = Int32Proxy.Deserialize(bytes);
			view.Tier = Int32Proxy.Deserialize(bytes);
			view.WeaponSecondaryAction = Int32Proxy.Deserialize(bytes);

			return view;
		}

		public static void Serialize(Stream stream, UberStrikeItemWeaponView instance)
		{
			int mask = 0;
			using (var bytes = new MemoryStream())
			{
				Int32Proxy.Serialize(bytes, instance.AccuracySpread);
				Int32Proxy.Serialize(bytes, instance.CombatRange);
				Int32Proxy.Serialize(bytes, instance.CriticalStrikeBonus);

				if (instance.CustomProperties != null)
					DictionaryProxy<string, string>.Serialize(bytes, instance.CustomProperties, new DictionaryProxy<string, string>.Serializer<string>(StringProxy.Serialize), new DictionaryProxy<string, string>.Serializer<string>(StringProxy.Serialize));
				else
					mask |= 1;

                Int32Proxy.Serialize(bytes, instance.DamageKnockback);
				Int32Proxy.Serialize(bytes, instance.DamagePerProjectile);
				Int32Proxy.Serialize(bytes, instance.DefaultZoomMultiplier);

				if (instance.Description != null)
					StringProxy.Serialize(bytes, instance.Description);
				else
					mask |= 2;

				BooleanProxy.Serialize(bytes, instance.HasAutomaticFire);
				Int32Proxy.Serialize(bytes, instance.ID);
				BooleanProxy.Serialize(bytes, instance.IsConsumable);
				EnumProxy<UberStrikeItemClass>.Serialize(bytes, instance.ItemClass);

				if (instance.ItemProperties != null)
					DictionaryProxy<ItemPropertyType, int>.Serialize(bytes, instance.ItemProperties, EnumProxy<ItemPropertyType>.Serialize, Int32Proxy.Serialize);
				else
					mask |= 4;

				Int32Proxy.Serialize(bytes, instance.LevelLock);
				Int32Proxy.Serialize(bytes, instance.MaxAmmo);
				Int32Proxy.Serialize(bytes, instance.MaxDurationDays);
				Int32Proxy.Serialize(bytes, instance.MaxZoomMultiplier);
				Int32Proxy.Serialize(bytes, instance.MinZoomMultiplier);
				Int32Proxy.Serialize(bytes, instance.MissileBounciness);
				Int32Proxy.Serialize(bytes, instance.MissileForceImpulse);
				Int32Proxy.Serialize(bytes, instance.MissileTimeToDetonate);

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

				Int32Proxy.Serialize(bytes, instance.ProjectileSpeed);
				Int32Proxy.Serialize(bytes, instance.ProjectilesPerShot);
				Int32Proxy.Serialize(bytes, instance.RateOfFire);
				Int32Proxy.Serialize(bytes, instance.RecoilKickback);
				Int32Proxy.Serialize(bytes, instance.RecoilMovement);
				Int32Proxy.Serialize(bytes, instance.SecondaryActionReticle);
				EnumProxy<ItemShopHighlightType>.Serialize(bytes, instance.ShopHighlightType);
				Int32Proxy.Serialize(bytes, instance.SplashRadius);
				Int32Proxy.Serialize(bytes, instance.StartAmmo);
				Int32Proxy.Serialize(bytes, instance.Tier);
				Int32Proxy.Serialize(bytes, instance.WeaponSecondaryAction);
				Int32Proxy.Serialize(stream, ~mask);
				bytes.WriteTo(stream);
			}
		}
	}
}
