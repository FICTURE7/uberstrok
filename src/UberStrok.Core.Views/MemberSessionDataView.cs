using System;
using UberStrok.Core.Common;

namespace UberStrok.Core.Views
{
    [Serializable]
    public class MemberSessionDataView
    {
        public string AuthToken { get; set; }
        public int Cmid { get; set; }
        public string Name { get; set; }
        public MemberAccessLevel AccessLevel { get; set; }
        public int Level { get; set; }
        public int XP { get; set; }
        public string ClanTag { get; set; }
        public ChannelType Channel { get; set; }
        public DateTime LoginDate { get; set; }
        public bool IsBanned { get; set; }
    }
}
