using System;
using System.IO;

namespace UberStrok.Core.Serialization
{
	public static class Int32Proxy
	{
		public static int Deserialize(Stream bytes)
		{
			byte[] buffer = new byte[4];
			bytes.Read(buffer, 0, 4);

			return BitConverter.ToInt32(buffer, 0);
		}

		public static void Serialize(Stream bytes, int instance)
		{
			byte[] buffer = BitConverter.GetBytes(instance);
			bytes.Write(buffer, 0, buffer.Length);
		}
	}
}
