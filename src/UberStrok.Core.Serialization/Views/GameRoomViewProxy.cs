using System.IO;
using UberStrok.Core.Views;

namespace UberStrok.Core.Serialization.Views
{
    public static class GameRoomViewProxy
	{
		public static GameRoomView Deserialize(Stream bytes)
		{
			int mask = Int32Proxy.Deserialize(bytes);
			var view = new GameRoomView();
			view.MapId = Int32Proxy.Deserialize(bytes);
			view.Number = Int32Proxy.Deserialize(bytes);

			if ((mask & 1) != 0)
				view.Server = ConnectionAddressViewProxy.Deserialize(bytes);

			return view;
		}

		public static void Serialize(Stream stream, GameRoomView instance)
		{
			int mask = 0;
			using (var bytes = new MemoryStream())
			{
				Int32Proxy.Serialize(bytes, instance.MapId);
				Int32Proxy.Serialize(bytes, instance.Number);

				if (instance.Server != null)
					ConnectionAddressViewProxy.Serialize(bytes, instance.Server);
				else
					mask |= 1;

				Int32Proxy.Serialize(stream, ~mask);
				bytes.WriteTo(stream);
			}
		}
	}
}
