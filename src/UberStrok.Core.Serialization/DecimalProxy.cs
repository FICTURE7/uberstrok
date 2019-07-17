using System.IO;

namespace UberStrok.Core.Serialization.Views
{
    public static class DecimalProxy
    {
        public static void Serialize(Stream bytes, decimal instance)
        {
            int[] bits = decimal.GetBits(instance);
            Int32Proxy.Serialize(bytes, bits[0]);
            Int32Proxy.Serialize(bytes, bits[1]);
            Int32Proxy.Serialize(bytes, bits[2]);
            Int32Proxy.Serialize(bytes, bits[3]);
        }

        public static decimal Deserialize(Stream bytes)
        {
            int[] bits = new int[] {
            Int32Proxy.Deserialize(bytes),
            Int32Proxy.Deserialize(bytes),
            Int32Proxy.Deserialize(bytes),
            Int32Proxy.Deserialize(bytes)
            };      
            return new decimal(bits);
        }
    }
}