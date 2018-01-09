using System;
using System.IO;
using UberStrok.Core.Common;

namespace UberStrok.Core.Serialization
{
    public static class ColorProxy
    {
        public static void Serialize(Stream bytes, Color instance)
        {
            bytes.Write(BitConverter.GetBytes(instance.R), 0, 4);
            bytes.Write(BitConverter.GetBytes(instance.G), 0, 4);
            bytes.Write(BitConverter.GetBytes(instance.B), 0, 4);
        }

        public static Color Deserialize(Stream bytes)
        {
            byte[] array = new byte[12];
            bytes.Read(array, 0, 12);
            return new Color(BitConverter.ToSingle(array, 0), BitConverter.ToSingle(array, 4), BitConverter.ToSingle(array, 8));
        }
    }
}
