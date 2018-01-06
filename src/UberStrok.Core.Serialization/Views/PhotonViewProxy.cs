using System.IO;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Core.Serialization.Views
{
    public static class PhotonViewProxy
	{
		public static PhotonView Deserialize(Stream bytes)
		{
			var mask = Int32Proxy.Deserialize(bytes);
			var view = new PhotonView();
			if ((mask & 1) != 0)
				view.IP = StringProxy.Deserialize(bytes);

            view.MinLatency = Int32Proxy.Deserialize(bytes);

			if ((mask & 2) != 0)
				view.Name = StringProxy.Deserialize(bytes);

			view.PhotonId = Int32Proxy.Deserialize(bytes);
			view.Port = Int32Proxy.Deserialize(bytes);
			view.Region = EnumProxy<RegionType>.Deserialize(bytes);
			view.UsageType = EnumProxy<PhotonUsageType>.Deserialize(bytes);
			return view;
		}

		public static void Serialize(Stream stream, PhotonView instance)
		{
			int mask = 0;
			using (var bytes = new MemoryStream())
			{
				if (instance.IP != null)
					StringProxy.Serialize(bytes, instance.IP);
				else
					mask |= 1;

                Int32Proxy.Serialize(bytes, instance.MinLatency);

                if (instance.Name != null)
					StringProxy.Serialize(bytes, instance.Name);
				else
					mask |= 2;

                Int32Proxy.Serialize(bytes, instance.PhotonId);
				Int32Proxy.Serialize(bytes, instance.Port);
				EnumProxy<RegionType>.Serialize(bytes, instance.Region);
				EnumProxy<PhotonUsageType>.Serialize(bytes, instance.UsageType);
				Int32Proxy.Serialize(stream, ~mask);
				bytes.WriteTo(stream);
			}
		}
	}
}
