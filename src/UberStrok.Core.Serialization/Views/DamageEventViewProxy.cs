using System.IO;
using UberStrok.Core.Views;

namespace UberStrok.Core.Serialization.Views
{
    public static class DamageEventViewProxy
    {
        public static void Serialize(Stream stream, DamageEventView instance)
        {
            int mask = 0;
            using (var bytes = new MemoryStream())
            {
                ByteProxy.Serialize(bytes, instance.BodyPartFlag);

                if (instance.Damage != null)
                    DictionaryProxy<byte, byte>.Serialize(bytes, instance.Damage, ByteProxy.Serialize, ByteProxy.Serialize);
                else
                    mask |= 1;

                Int32Proxy.Serialize(bytes, instance.DamageEffectFlag);
                SingleProxy.Serialize(bytes, instance.DamgeEffectValue);
                Int32Proxy.Serialize(stream, ~mask);
                bytes.WriteTo(stream);
            }
        }

        public static DamageEventView Deserialize(Stream bytes)
        {
            int mask = Int32Proxy.Deserialize(bytes);
            var instance = new DamageEventView();

            instance.BodyPartFlag = ByteProxy.Deserialize(bytes);

            if ((mask & 1) != 0)
                instance.Damage = DictionaryProxy<byte, byte>.Deserialize(bytes, ByteProxy.Deserialize, ByteProxy.Deserialize);

            instance.DamageEffectFlag = Int32Proxy.Deserialize(bytes);
            instance.DamgeEffectValue = SingleProxy.Deserialize(bytes);
            return instance;
        }
    }
}
