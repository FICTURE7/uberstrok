using System;
using System.IO;

namespace UberStrok.Core.Serialization
{
	public static class DateTimeProxy
	{
		public static DateTime Deserialize(Stream bytes)
		{
			byte[] buffer = new byte[8];
			bytes.Read(buffer, 0, 8);

			return new DateTime(BitConverter.ToInt64(buffer, 0));
		}

		public static void Serialize(Stream bytes, DateTime instance)
		{
			byte[] buffer = BitConverter.GetBytes(instance.Ticks);
			bytes.Write(buffer, 0, buffer.Length);
		}
	}
}
