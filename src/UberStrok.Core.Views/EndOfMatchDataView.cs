using System;
using System.Collections.Generic;

namespace UberStrok.Core.Views
{
    [Serializable]
    public class EndOfMatchDataView
    {
        /* "wrongly spelt efficient but it's like that in client /shrug" - SirAnuse */

        public List<StatsSummaryView> MostValuablePlayers { get; set; }
        public int MostEffecientWeaponId { get; set; }
        public StatsCollectionView PlayerStatsTotal { get; set; }
        public StatsCollectionView PlayerStatsBestPerLife { get; set; }
        public Dictionary<byte, ushort> PlayerXpEarned { get; set; }
        public int TimeInGameMinutes { get; set; }
        public bool HasWonMatch { get; set; }
        public string MatchGuid { get; set; }
    }
}