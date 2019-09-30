using System;
using System.Text;

namespace UberStrok.Core.Views
{
	[Serializable]
	public class PlayerPersonalRecordStatisticsView
	{
		public PlayerPersonalRecordStatisticsView()
		{
            // Space
		}

		public PlayerPersonalRecordStatisticsView(
            int mostHeadshots,
            int mostNutshots,
            int mostConsecutiveSnipes,
            int mostXPEarned,
            int mostSplats,
            int mostDamageDealt,
            int mostDamageReceived,
            int mostArmorPickedUp,
            int mostHealthPickedUp,
            int mostMeleeSplats,
            int mostMachinegunSplats,
            int mostShotgunSplats,
            int mostSniperSplats,
            int mostSplattergunSplats,
            int mostCannonSplats,
            int mostLauncherSplats)
		{
			MostArmorPickedUp = mostArmorPickedUp;
			MostCannonSplats = mostCannonSplats;
			MostConsecutiveSnipes = mostConsecutiveSnipes;
			MostDamageDealt = mostDamageDealt;
			MostDamageReceived = mostDamageReceived;
			MostHeadshots = mostHeadshots;
			MostHealthPickedUp = mostHealthPickedUp;
			MostLauncherSplats = mostLauncherSplats;
			MostMachinegunSplats = mostMachinegunSplats;
			MostMeleeSplats = mostMeleeSplats;
			MostNutshots = mostNutshots;
			MostShotgunSplats = mostShotgunSplats;
			MostSniperSplats = mostSniperSplats;
			MostSplats = mostSplats;
			MostSplattergunSplats = mostSplattergunSplats;
			MostXPEarned = mostXPEarned;
		}

		public override string ToString()
		{
			var builder = new StringBuilder().Append("[PlayerPersonalRecordStatisticsView: ")
                .Append("[MostArmorPickedUp: ").Append(MostArmorPickedUp)
                .Append("][MostCannonSplats: ").Append(MostCannonSplats)
                .Append("][MostConsecutiveSnipes: ").Append(MostConsecutiveSnipes)
                .Append("][MostDamageDealt: ").Append(MostDamageDealt)
                .Append("][MostDamageReceived: ").Append(MostDamageReceived)
                .Append("][MostHeadshots: ").Append(MostHeadshots)
                .Append("][MostHealthPickedUp: ").Append(MostHealthPickedUp)
                .Append("][MostLauncherSplats: ").Append(MostLauncherSplats)
                .Append("][MostMachinegunSplats: ").Append(MostMachinegunSplats)
                .Append("][MostMeleeSplats: ").Append(MostMeleeSplats)
                .Append("][MostNutshots: ").Append(MostNutshots)
                .Append("][MostShotgunSplats: ").Append(MostShotgunSplats)
                .Append("][MostSniperSplats: ").Append(MostSniperSplats)
                .Append("][MostSplats: ").Append(MostSplats)
                .Append("][MostSplattergunSplats: ").Append(MostSplattergunSplats)
                .Append("][MostXPEarned: ").Append(MostXPEarned)
			.Append("]]");
			return builder.ToString();
		}

		public int MostArmorPickedUp { get; set; }
		public int MostCannonSplats { get; set; }
		public int MostConsecutiveSnipes { get; set; }
		public int MostDamageDealt { get; set; }
		public int MostDamageReceived { get; set; }
		public int MostHeadshots { get; set; }
		public int MostHealthPickedUp { get; set; }
		public int MostLauncherSplats { get; set; }
		public int MostMachinegunSplats { get; set; }
		public int MostMeleeSplats { get; set; }
		public int MostNutshots { get; set; }
		public int MostShotgunSplats { get; set; }
		public int MostSniperSplats { get; set; }
		public int MostSplats { get; set; }
		public int MostSplattergunSplats { get; set; }
		public int MostXPEarned { get; set; }
	}
}
