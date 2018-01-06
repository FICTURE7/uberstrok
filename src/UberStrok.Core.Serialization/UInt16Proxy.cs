using System;
using System.IO;

namespace UberStrok.Core.Serialization
{
    // Apparently there is also a UShortProxy, but they are same thing.

	public static class UInt16Proxy
	{
		public static ushort Deserialize(Stream bytes)
		{
			byte[] buffer = new byte[2];
			bytes.Read(buffer, 0, 2);
			return BitConverter.ToUInt16(buffer, 0);
		}

		public static void Serialize(Stream bytes, ushort instance)
		{
			byte[] buffer = BitConverter.GetBytes(instance);
			bytes.Write(buffer, 0, buffer.Length);
		}
	}
}
