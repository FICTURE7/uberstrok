using System.IO;
using UbzStuff.Core.Common;
using UbzStuff.Core.Views;

namespace UbzStuff.Core.Serialization.Views
{
    public static class MemberAuthenticationResultViewProxy
	{
		public static MemberAuthenticationResultView Deserialize(Stream bytes)
		{
			int mask = Int32Proxy.Deserialize(bytes);
			var view = new MemberAuthenticationResultView();
			if ((mask & 1) != 0)
				view.AuthToken = StringProxy.Deserialize(bytes);

            view.IsAccountComplete = BooleanProxy.Deserialize(bytes);

            if ((mask & 2) != 0)
				view.LuckyDraw = LuckyDrawUnityViewProxy.Deserialize(bytes);

            view.MemberAuthenticationResult = EnumProxy<MemberAuthenticationResult>.Deserialize(bytes);

            if ((mask & 4) != 0)
				view.MemberView = MemberViewProxy.Deserialize(bytes);
			if ((mask & 8) != 0)
				view.PlayerStatisticsView = PlayerStatisticsViewProxy.Deserialize(bytes);

            view.ServerTime = DateTimeProxy.Deserialize(bytes);
			return view;
		}

		public static void Serialize(Stream stream, MemberAuthenticationResultView instance)
		{
			int mask = 0;
			using (var bytes = new MemoryStream())
			{
				if (instance.AuthToken != null)
					StringProxy.Serialize(bytes, instance.AuthToken);
				else
					mask |= 1;

                BooleanProxy.Serialize(bytes, instance.IsAccountComplete);

                if (instance.LuckyDraw != null)
					LuckyDrawUnityViewProxy.Serialize(bytes, instance.LuckyDraw);
				else
					mask |= 2;

                EnumProxy<MemberAuthenticationResult>.Serialize(bytes, instance.MemberAuthenticationResult);

                if (instance.MemberView != null)
					MemberViewProxy.Serialize(bytes, instance.MemberView);
				else
					mask |= 4;
				if (instance.PlayerStatisticsView != null)
					PlayerStatisticsViewProxy.Serialize(bytes, instance.PlayerStatisticsView);
				else
					mask |= 8;

                DateTimeProxy.Serialize(bytes, instance.ServerTime);
				Int32Proxy.Serialize(stream, ~mask);
				bytes.WriteTo(stream);
			}
		}
	}
}
