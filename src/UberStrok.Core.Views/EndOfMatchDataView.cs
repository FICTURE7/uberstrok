using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UberStrok.Core.Common;

namespace UberStrok.Core.Views
{
    [Serializable]
    public class EndOfMatchDataView
    {
        public List<StatsSummaryView> MostValuablePlayers { get; set; }
        // wrongly spelt efficient but it's like that in client /shrug
        public int MostEffecientWeaponId { get; set; }
        public StatsCollectionView PlayerStatsTotal { get; set; }
        public StatsCollectionView PlayerStatsBestPerLife { get; set; }
        public Dictionary<byte, ushort> PlayerXpEarned { get; set; }
        public int TimeInGameMinutes { get; set; }
        public bool HasWonMatch { get; set; }
        public string MatchGuid { get; set; }
    }
}