using System;
using System.IO;

namespace UberStrok.Core.Serialization
{
	public static class SingleProxy
	{
		public static float Deserialize(Stream bytes)
		{
			byte[] buffer = new byte[4];
			bytes.Read(buffer, 0, 4);
			return BitConverter.ToSingle(buffer, 0);
		}

		public static void Serialize(Stream bytes, float instance)
		{
			byte[] buffer = BitConverter.GetBytes(instance);
			bytes.Write(buffer, 0, buffer.Length);
		}
	}
}
