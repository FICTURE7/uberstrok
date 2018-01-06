using System.IO;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Core.Serialization.Views
{
    public static class ServerConnectionViewProxy
	{
		public static ServerConnectionView Deserialize(Stream bytes)
		{
			int mask = Int32Proxy.Deserialize(bytes);
			var view = new ServerConnectionView();
			view.AccessLevel = EnumProxy<MemberAccessLevel>.Deserialize(bytes);

			if ((mask & 1) != 0)
				view.ApiVersion = StringProxy.Deserialize(bytes);

			view.Channel = EnumProxy<ChannelType>.Deserialize(bytes);
			view.Cmid = Int32Proxy.Deserialize(bytes);
			return view;
		}

		public static void Serialize(Stream stream, ServerConnectionView instance)
		{
			int mask = 0;
			using (var bytes = new MemoryStream())
			{
				EnumProxy<MemberAccessLevel>.Serialize(bytes, instance.AccessLevel);

				if (instance.ApiVersion != null)
					StringProxy.Serialize(bytes, instance.ApiVersion);
				else
					mask |= 1;

				EnumProxy<ChannelType>.Serialize(bytes, instance.Channel);
				Int32Proxy.Serialize(bytes, instance.Cmid);
				Int32Proxy.Serialize(stream, ~mask);
				bytes.WriteTo(stream);
			}
		}
	}
}
