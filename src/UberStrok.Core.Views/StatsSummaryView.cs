using System;
using System.Collections.Generic;
using UberStrok.Core.Common;

namespace UberStrok.Core.Views
{
    [Serializable]
    public class StatsSummaryView
    {
        public string Name { get; set; }
        public int Kills { get; set; }
        public int Deaths { get; set; }
        public int Level { get; set; }
        public int Cmid { get; set; }
        public TeamID Team { get; set; }
        public Dictionary<byte, ushort> Achievements { get; set; }
    }
}