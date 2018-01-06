using System.Collections.Generic;
using System.IO;

namespace UberStrok.Core.Serialization
{
    public static class ListProxy<T>
	{
		public static List<T> Deserialize(Stream bytes, Deserializer<T> serialization)
		{
			ushort length = UInt16Proxy.Deserialize(bytes);
			List<T> list = new List<T>(length);
			for (int i = 0; i < length; i++)
				list.Add(serialization(bytes));

			return list;
		}

		public static void Serialize(Stream bytes, ICollection<T> instance, Serializer<T> serialization)
		{
			UInt16Proxy.Serialize(bytes, (ushort)instance.Count);
			foreach (T current in instance)
				serialization(bytes, current);
		}

		public delegate U Deserializer<U>(Stream stream);

		public delegate void Serializer<U>(Stream stream, U instance);
	}
}
