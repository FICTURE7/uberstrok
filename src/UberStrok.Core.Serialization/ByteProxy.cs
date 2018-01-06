using System.IO;

namespace UberStrok.Core.Serialization
{
    public static class ByteProxy
	{
		public static byte Deserialize(Stream bytes)
		{
			return (byte)bytes.ReadByte();
		}

		public static void Serialize(Stream bytes, byte instance)
		{
			bytes.WriteByte(instance);
		}
	}
}
