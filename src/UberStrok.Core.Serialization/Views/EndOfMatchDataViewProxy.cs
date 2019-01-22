using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UberStrok.Core.Views;

namespace UberStrok.Core.Serialization.Views
{
    public static class EndOfMatchDataViewProxy
    {
        // Token: 0x060010BF RID: 4287 RVA: 0x00016980 File Offset: 0x00014B80
        public static void Serialize(Stream stream, EndOfMatchDataView instance)
        {
            int num = 0;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                BooleanProxy.Serialize(memoryStream, instance.HasWonMatch);
                if (instance.MatchGuid != null)
                {
                    StringProxy.Serialize(memoryStream, instance.MatchGuid);
                }
                else
                {
                    num |= 1;
                }
                // it's spelt effecient in the client too
                Int32Proxy.Serialize(memoryStream, instance.MostEffecientWeaponId);
                if (instance.MostValuablePlayers != null)
                {
                    ListProxy<StatsSummaryView>.Serialize(memoryStream, instance.MostValuablePlayers, new ListProxy<StatsSummaryView>.Serializer<StatsSummaryView>(StatsSummaryViewProxy.Serialize));
                }
                else
                {
                    num |= 2;
                }
                if (instance.PlayerStatsBestPerLife != null)
                {
                    StatsCollectionViewProxy.Serialize(memoryStream, instance.PlayerStatsBestPerLife);
                }
                else
                {
                    num |= 4;
                }
                if (instance.PlayerStatsTotal != null)
                {
                    StatsCollectionViewProxy.Serialize(memoryStream, instance.PlayerStatsTotal);
                }
                else
                {
                    num |= 8;
                }
                if (instance.PlayerXpEarned != null)
                {
                    DictionaryProxy<byte, ushort>.Serialize(memoryStream, instance.PlayerXpEarned, new DictionaryProxy<byte, ushort>.Serializer<byte>(ByteProxy.Serialize), new DictionaryProxy<byte, ushort>.Serializer<ushort>(UInt16Proxy.Serialize));
                }
                else
                {
                    num |= 16;
                }
                Int32Proxy.Serialize(memoryStream, instance.TimeInGameMinutes);
                Int32Proxy.Serialize(stream, ~num);
                memoryStream.WriteTo(stream);
            }
        }

        // Token: 0x060010C0 RID: 4288 RVA: 0x00016AB0 File Offset: 0x00014CB0
        public static EndOfMatchDataView Deserialize(Stream bytes)
        {
            int num = Int32Proxy.Deserialize(bytes);
            EndOfMatchDataView endOfMatchData = new EndOfMatchDataView();
            endOfMatchData.HasWonMatch = BooleanProxy.Deserialize(bytes);
            if ((num & 1) != 0)
            {
                endOfMatchData.MatchGuid = StringProxy.Deserialize(bytes);
            }
            endOfMatchData.MostEffecientWeaponId = Int32Proxy.Deserialize(bytes);
            if ((num & 2) != 0)
            {
                endOfMatchData.MostValuablePlayers = ListProxy<StatsSummaryView>.Deserialize(bytes, new ListProxy<StatsSummaryView>.Deserializer<StatsSummaryView>(StatsSummaryViewProxy.Deserialize));
            }
            if ((num & 4) != 0)
            {
                endOfMatchData.PlayerStatsBestPerLife = StatsCollectionViewProxy.Deserialize(bytes);
            }
            if ((num & 8) != 0)
            {
                endOfMatchData.PlayerStatsTotal = StatsCollectionViewProxy.Deserialize(bytes);
            }
            if ((num & 16) != 0)
            {
                endOfMatchData.PlayerXpEarned = DictionaryProxy<byte, ushort>.Deserialize(bytes, new DictionaryProxy<byte, ushort>.Deserializer<byte>(ByteProxy.Deserialize), new DictionaryProxy<byte, ushort>.Deserializer<ushort>(UInt16Proxy.Deserialize));
            }
            endOfMatchData.TimeInGameMinutes = Int32Proxy.Deserialize(bytes);
            return endOfMatchData;
        }
    }
}