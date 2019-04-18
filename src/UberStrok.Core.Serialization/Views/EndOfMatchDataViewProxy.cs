using System.IO;
using UberStrok.Core.Views;

namespace UberStrok.Core.Serialization.Views
{
    public static class EndOfMatchDataViewProxy
    {
        public static EndOfMatchDataView Deserialize(Stream bytes)
        {
            int mask = Int32Proxy.Deserialize(bytes);
            var instance = new EndOfMatchDataView();
            instance.HasWonMatch = BooleanProxy.Deserialize(bytes);

            if ((mask & 1) != 0)
                instance.MatchGuid = StringProxy.Deserialize(bytes);

            instance.MostEffecientWeaponId = Int32Proxy.Deserialize(bytes);

            if ((mask & 2) != 0)
                instance.MostValuablePlayers = ListProxy<StatsSummaryView>.Deserialize(bytes, new ListProxy<StatsSummaryView>.Deserializer<StatsSummaryView>(StatsSummaryViewProxy.Deserialize));
            if ((mask & 4) != 0)
                instance.PlayerStatsBestPerLife = StatsCollectionViewProxy.Deserialize(bytes);
            if ((mask & 8) != 0)
                instance.PlayerStatsTotal = StatsCollectionViewProxy.Deserialize(bytes);
            if ((mask & 16) != 0)
                instance.PlayerXpEarned = DictionaryProxy<byte, ushort>.Deserialize(bytes, new DictionaryProxy<byte, ushort>.Deserializer<byte>(ByteProxy.Deserialize), new DictionaryProxy<byte, ushort>.Deserializer<ushort>(UInt16Proxy.Deserialize));

            instance.TimeInGameMinutes = Int32Proxy.Deserialize(bytes);
            return instance;
        }

        public static void Serialize(Stream stream, EndOfMatchDataView instance)
        {
            int mask = 0;
            using (var bytes = new MemoryStream())
            {
                BooleanProxy.Serialize(bytes, instance.HasWonMatch);
                if (instance.MatchGuid != null)
                    StringProxy.Serialize(bytes, instance.MatchGuid);
                else
                    mask |= 1;

                Int32Proxy.Serialize(bytes, instance.MostEffecientWeaponId);
                if (instance.MostValuablePlayers != null)
                    ListProxy<StatsSummaryView>.Serialize(bytes, instance.MostValuablePlayers, new ListProxy<StatsSummaryView>.Serializer<StatsSummaryView>(StatsSummaryViewProxy.Serialize));
                else
                    mask |= 2;
                
                if (instance.PlayerStatsBestPerLife != null)
                    StatsCollectionViewProxy.Serialize(bytes, instance.PlayerStatsBestPerLife);
                else
                    mask |= 4;

                if (instance.PlayerStatsTotal != null)
                    StatsCollectionViewProxy.Serialize(bytes, instance.PlayerStatsTotal);
                else
                    mask |= 8;

                if (instance.PlayerXpEarned != null)
                    DictionaryProxy<byte, ushort>.Serialize(bytes, instance.PlayerXpEarned, new DictionaryProxy<byte, ushort>.Serializer<byte>(ByteProxy.Serialize), new DictionaryProxy<byte, ushort>.Serializer<ushort>(UInt16Proxy.Serialize));
                else
                    mask |= 16;

                Int32Proxy.Serialize(bytes, instance.TimeInGameMinutes);
                Int32Proxy.Serialize(stream, ~mask);
                bytes.WriteTo(stream);
            }
        }
    }
}