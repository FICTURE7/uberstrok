using System;
using System.IO;
using UberStrok.Core.Common;

namespace UberStrok.Core.Serialization.Common
{
    public static class Vector3Proxy
    {
        public static void Serialize(Stream bytes, Vector3Old instance)
        {
            bytes.Write(BitConverter.GetBytes(instance.x), 0, 4);
            bytes.Write(BitConverter.GetBytes(instance.y), 0, 4);
            bytes.Write(BitConverter.GetBytes(instance.z), 0, 4);
        }

        public static Vector3Old Deserialize(Stream bytes)
        {
            byte[] data = new byte[12];
            bytes.Read(data, 0, 12);
            return new Vector3Old(BitConverter.ToSingle(data, 0), BitConverter.ToSingle(data, 4), BitConverter.ToSingle(data, 8));
        }
    }
}
