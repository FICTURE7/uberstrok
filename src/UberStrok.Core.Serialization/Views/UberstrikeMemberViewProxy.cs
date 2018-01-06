using System.IO;
using UberStrok.Core.Views;

namespace UberStrok.Core.Serialization.Views
{
    public static class UberstrikeMemberViewProxy
	{
		public static UberstrikeMemberView Deserialize(Stream bytes)
		{
			int mask = Int32Proxy.Deserialize(bytes);
			var view = new UberstrikeMemberView();

			if ((mask & 1) != 0)
				view.PlayerCardView = PlayerCardViewProxy.Deserialize(bytes);
			if ((mask & 2) != 0)
				view.PlayerStatisticsView = PlayerStatisticsViewProxy.Deserialize(bytes);

			return view;
		}

		public static void Serialize(Stream stream, UberstrikeMemberView instance)
		{
			int mask = 0;
			using (var bytes = new MemoryStream())
			{
				if (instance.PlayerCardView != null)
					PlayerCardViewProxy.Serialize(bytes, instance.PlayerCardView);
				else
					mask |= 1;
				if (instance.PlayerStatisticsView != null)
					PlayerStatisticsViewProxy.Serialize(bytes, instance.PlayerStatisticsView);
				else
					mask |= 2;

                Int32Proxy.Serialize(stream, ~mask);
				bytes.WriteTo(stream);
			}
		}
	}
}
