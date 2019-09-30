using System;
using UberStrok.Core.Common;

namespace UberStrok.WebServices.AspNetCore.Models
{
    public class Member
    {
        [Flags]
        public enum LoadOptions
        {
            Socials = 1,
            Transactions = 2,
            Inventory = 4,
            Loadout = 8,
            Statistics = 16,

            All = Socials | Transactions | Inventory | Loadout | Statistics
        }

        public int Id { get; set; }
        public int? ClanId { get; set; }
        public Clan Clan { get; set; }
        public string SteamId { get; set; }
        public string Name { get; set; }
        public int Points { get; set; }
        public int Credits { get; set; }
        public int Level { get; set; }
        public int Xp { get; set; }
		public long Shots { get; set; }
		public int Nutshots { get; set; }
		public int Headshots { get; set; }
		public long Hits { get; set; }
		public int Splats { get; set; }
		public int Splatted { get; set; }
        public DateTime LastLogin { get; set; }
        public DateTime LastModify { get; set; }
        public DateTime? MuteExpiration { get; set; }
        public DateTime? BanExpiration { get; set; }
        public MemberAccessLevel AccessLevel { get; set; }
        public EmailAddressStatus EmailStatus { get; set; }

        public MemberSocials Socials { get; set; }
        public MemberTransactions Transactions { get; set; }
        public MemberInventory Inventory { get; set; }
        public MemberLoadout Loadout { get; set; }
        public MemberStatistics Statistics { get; set; }

        public override string ToString()
            => $"({Id}:\"{SteamId ?? "null"}\" -> \"{Name ?? "null"}\")";
    }
}
