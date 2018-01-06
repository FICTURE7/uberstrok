using System.IO;
using UberStrok.Core.Views;

namespace UberStrok.Core.Serialization.Views
{
    public static class PlayerCardViewProxy
	{
		public static PlayerCardView Deserialize(Stream bytes)
		{
			int mask = Int32Proxy.Deserialize(bytes);
			var view = new PlayerCardView();
			view.Cmid = Int32Proxy.Deserialize(bytes);
			view.Hits = Int64Proxy.Deserialize(bytes);

			if ((mask & 1) != 0)
				view.Name = StringProxy.Deserialize(bytes);
			if ((mask & 2) != 0)
				view.Precision = StringProxy.Deserialize(bytes);

			view.Ranking = Int32Proxy.Deserialize(bytes);
			view.Shots = Int64Proxy.Deserialize(bytes);
			view.Splats = Int32Proxy.Deserialize(bytes);
			view.Splatted = Int32Proxy.Deserialize(bytes);

			if ((mask & 4) != 0)
				view.TagName = StringProxy.Deserialize(bytes);

			return view;
		}

		public static void Serialize(Stream stream, PlayerCardView instance)
		{
			int mask = 0;
			using (var bytes = new MemoryStream())
			{
				Int32Proxy.Serialize(bytes, instance.Cmid);
				Int64Proxy.Serialize(bytes, instance.Hits);

				if (instance.Name != null)
					StringProxy.Serialize(bytes, instance.Name);
				else
					mask |= 1;
				if (instance.Precision != null)
					StringProxy.Serialize(bytes, instance.Precision);
				else
					mask |= 2;

				Int32Proxy.Serialize(bytes, instance.Ranking);
				Int64Proxy.Serialize(bytes, instance.Shots);
				Int32Proxy.Serialize(bytes, instance.Splats);
				Int32Proxy.Serialize(bytes, instance.Splatted);

				if (instance.TagName != null)
					StringProxy.Serialize(bytes, instance.TagName);
				else
					mask |= 4;

				Int32Proxy.Serialize(stream, ~mask);
				bytes.WriteTo(stream);
			}
		}
	}
}
