using System.IO;
using UberStrok.Core.Views;

namespace UberStrok.Core.Serialization.Views
{
    public static class LuckyDrawSetUnityViewProxy
	{
		public static LuckyDrawSetUnityView Deserialize(Stream bytes)
		{
			var mask = Int32Proxy.Deserialize(bytes);
			var view = new LuckyDrawSetUnityView();
			view.CreditsAttributed = Int32Proxy.Deserialize(bytes);
			view.ExposeItemsToPlayers = BooleanProxy.Deserialize(bytes);
			view.Id = Int32Proxy.Deserialize(bytes);

			if ((mask & 1) != 0)
				view.ImageUrl = StringProxy.Deserialize(bytes);

            view.LuckyDrawId = Int32Proxy.Deserialize(bytes);

            if ((mask & 2) != 0)
				view.LuckyDrawSetItems = ListProxy<BundleItemView>.Deserialize(bytes, BundleItemViewProxy.Deserialize);

            view.PointsAttributed = Int32Proxy.Deserialize(bytes);
			view.SetWeight = Int32Proxy.Deserialize(bytes);
			return view;
		}

		public static void Serialize(Stream stream, LuckyDrawSetUnityView instance)
		{
			int mask = 0;
			using (var bytes = new MemoryStream())
			{
				Int32Proxy.Serialize(bytes, instance.CreditsAttributed);
				BooleanProxy.Serialize(bytes, instance.ExposeItemsToPlayers);
				Int32Proxy.Serialize(bytes, instance.Id);

                if (instance.ImageUrl != null)
					StringProxy.Serialize(bytes, instance.ImageUrl);
				else
					mask |= 1;

                Int32Proxy.Serialize(bytes, instance.LuckyDrawId);

                if (instance.LuckyDrawSetItems != null)
					ListProxy<BundleItemView>.Serialize(bytes, instance.LuckyDrawSetItems, BundleItemViewProxy.Serialize);
				else
					mask |= 2;

                Int32Proxy.Serialize(bytes, instance.PointsAttributed);
				Int32Proxy.Serialize(bytes, instance.SetWeight);
				Int32Proxy.Serialize(stream, ~mask);
				bytes.WriteTo(stream);
			}
		}
	}
}
