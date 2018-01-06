using System;
using System.IO;

namespace UberStrok.Core.Serialization
{
	public static class BooleanProxy
	{
		public static bool Deserialize(Stream bytes)
		{
			byte[] buffer = new byte[1];
			bytes.Read(buffer, 0, 1);

			return BitConverter.ToBoolean(buffer, 0);
		}

		public static void Serialize(Stream bytes, bool instance)
		{
			byte[] buffer = BitConverter.GetBytes(instance);
			bytes.Write(buffer, 0, buffer.Length);
		}
	}
}
