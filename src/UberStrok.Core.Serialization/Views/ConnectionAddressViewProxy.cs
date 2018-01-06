using System.IO;
using UberStrok.Core.Views;

namespace UberStrok.Core.Serialization.Views
{
    public static class ConnectionAddressViewProxy
	{
		public static ConnectionAddressView Deserialize(Stream bytes)
		{
			return new ConnectionAddressView
			{
				Ipv4 = Int32Proxy.Deserialize(bytes),
				Port = UInt16Proxy.Deserialize(bytes)
			};
		}

		public static void Serialize(Stream stream, ConnectionAddressView instance)
		{
			using (var bytes = new MemoryStream())
			{
				Int32Proxy.Serialize(bytes, instance.Ipv4);
				UInt16Proxy.Serialize(bytes, instance.Port);
				bytes.WriteTo(stream);
			}
		}
	}
}
