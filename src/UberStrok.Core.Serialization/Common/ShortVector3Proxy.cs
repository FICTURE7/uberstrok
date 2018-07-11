using System;
using System.IO;
using UberStrok.Core.Common;

namespace UberStrok.Core.Serialization.Common
{
    public static class ShortVector3Proxy
    {
        public static void Serialize(Stream bytes, Vector3Old instance)
        {
            bytes.Write(BitConverter.GetBytes((short)MathUtils.Clamp(instance.x * 100f, -32768f, 32767f)), 0, 2);
            bytes.Write(BitConverter.GetBytes((short)MathUtils.Clamp(instance.y * 100f, -32768f, 32767f)), 0, 2);
            bytes.Write(BitConverter.GetBytes((short)MathUtils.Clamp(instance.z * 100f, -32768f, 32767f)), 0, 2);
        }

        public static Vector3Old Deserialize(Stream bytes)
        {
            var array = new byte[6];
            bytes.Read(array, 0, 6);
            return new Vector3Old(0.01f * BitConverter.ToInt16(array, 0), 0.01f * BitConverter.ToInt16(array, 2), 0.01f * BitConverter.ToInt16(array, 4));
        }
    }
}
