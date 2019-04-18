using System.IO;
using UberStrok.Core.Views;
using UberStrok.Core.Common;

namespace UberStrok.Core.Serialization.Views
{
    public static class StatsSummaryViewProxy
    {
        public static StatsSummaryView Deserialize(Stream bytes)
        {
            int mask = Int32Proxy.Deserialize(bytes);
            var view = new StatsSummaryView();

            if ((mask & 1) != 0)
                view.Achievements = DictionaryProxy<byte, ushort>.Deserialize(bytes, new DictionaryProxy<byte, ushort>.Deserializer<byte>(ByteProxy.Deserialize), new DictionaryProxy<byte, ushort>.Deserializer<ushort>(UInt16Proxy.Deserialize));

            view.Cmid = Int32Proxy.Deserialize(bytes);
            view.Deaths = Int32Proxy.Deserialize(bytes);
            view.Kills = Int32Proxy.Deserialize(bytes);
            view.Level = Int32Proxy.Deserialize(bytes);

            if ((mask & 2) != 0)
                view.Name = StringProxy.Deserialize(bytes);

            view.Team = EnumProxy<TeamID>.Deserialize(bytes);
            return view;
        }

        public static void Serialize(Stream stream, StatsSummaryView instance)
        {
            int mask = 0;
            using (var bytes = new MemoryStream())
            {
                if (instance.Achievements != null)
                    DictionaryProxy<byte, ushort>.Serialize(bytes, instance.Achievements, new DictionaryProxy<byte, ushort>.Serializer<byte>(ByteProxy.Serialize), new DictionaryProxy<byte, ushort>.Serializer<ushort>(UInt16Proxy.Serialize));
                else
                    mask |= 1;

                Int32Proxy.Serialize(bytes, instance.Cmid);
                Int32Proxy.Serialize(bytes, instance.Deaths);
                Int32Proxy.Serialize(bytes, instance.Kills);
                Int32Proxy.Serialize(bytes, instance.Level);

                if (instance.Name != null)
                    StringProxy.Serialize(bytes, instance.Name);
                else
                    mask |= 2;

                EnumProxy<TeamID>.Serialize(bytes, instance.Team);
                Int32Proxy.Serialize(stream, ~mask);
                bytes.WriteTo(stream);
            }
        }
    }
}