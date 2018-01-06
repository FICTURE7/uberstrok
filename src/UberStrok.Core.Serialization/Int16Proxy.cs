using System;
using System.IO;

namespace UberStrok.Core.Serialization
{
	public static class Int16Proxy
	{
		public static short Deserialize(Stream bytes)
		{
			byte[] buffer = new byte[2];
			bytes.Read(buffer, 0, 2);

			return BitConverter.ToInt16(buffer, 0);
		}

		public static void Serialize(Stream bytes, short instance)
		{
			byte[] buffer = BitConverter.GetBytes(instance);
			bytes.Write(buffer, 0, buffer.Length);
		}
	}
}
