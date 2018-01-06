using System;
using System.IO;

namespace UberStrok.Core.Serialization
{
	public static class Int64Proxy
	{
		public static long Deserialize(Stream bytes)
		{
			byte[] buffer = new byte[8];
			bytes.Read(buffer, 0, 8);

			return BitConverter.ToInt64(buffer, 0);
		}

		public static void Serialize(Stream bytes, long instance)
		{
			byte[] bytes2 = BitConverter.GetBytes(instance);
			bytes.Write(bytes2, 0, bytes2.Length);
		}
	}
}
