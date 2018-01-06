using System;
using System.IO;

namespace UberStrok.Core.Serialization
{
	public static class EnumProxy<T>
	{
		public static T Deserialize(Stream bytes)
		{
			byte[] buffer = new byte[4];
			bytes.Read(buffer, 0, 4);

			return (T)Enum.ToObject(typeof(T), BitConverter.ToInt32(buffer, 0));
		}

		public static void Serialize(Stream bytes, T instance)
		{
			byte[] buffer = BitConverter.GetBytes(Convert.ToInt32(instance));
			bytes.Write(buffer, 0, buffer.Length);
		}
	}
}
