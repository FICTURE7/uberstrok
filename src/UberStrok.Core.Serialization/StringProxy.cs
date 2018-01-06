using System.IO;
using System.Text;

namespace UberStrok.Core.Serialization
{
    public static class StringProxy
	{
		public static string Deserialize(Stream bytes)
		{
			ushort length = UInt16Proxy.Deserialize(bytes);
			if (length > 0)
			{
				byte[] buffer = new byte[(length * 2)];
				bytes.Read(buffer, 0, buffer.Length);

				return Encoding.Unicode.GetString(buffer, 0, buffer.Length);
			}
			return string.Empty;
		}

		public static void Serialize(Stream bytes, string instance)
		{
			if (string.IsNullOrEmpty(instance))
			{
                UInt16Proxy.Serialize(bytes, 0);
			}
			else
			{
                UInt16Proxy.Serialize(bytes, (ushort)instance.Length);

				byte[] buffer = Encoding.Unicode.GetBytes(instance);
				bytes.Write(buffer, 0, buffer.Length);
			}
		}
	}
}
