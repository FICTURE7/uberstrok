using System.Collections.Generic;
using System.IO;

namespace UberStrok.Core.Serialization
{
    public static class DictionaryProxy<TKey, TValue>
	{
		public static Dictionary<TKey, TValue> Deserialize(Stream bytes, Deserializer<TKey> keySerialization, Deserializer<TValue> valueSerialization)
		{
			int length = Int32Proxy.Deserialize(bytes);
			var dictionary = new Dictionary<TKey, TValue>(length);
			for (int i = 0; i < length; i++)
				dictionary.Add(keySerialization(bytes), valueSerialization(bytes));

			return dictionary;
		}

		public static void Serialize(Stream bytes, Dictionary<TKey, TValue> instance, Serializer<TKey> keySerialization, Serializer<TValue> valueSerialization)
		{
			Int32Proxy.Serialize(bytes, instance.Count);
			foreach (KeyValuePair<TKey, TValue> current in instance)
			{
				keySerialization(bytes, current.Key);
				valueSerialization(bytes, current.Value);
			}
		}

		public delegate T Deserializer<T>(Stream stream);

		public delegate void Serializer<T>(Stream stream, T instance);
	}
}
