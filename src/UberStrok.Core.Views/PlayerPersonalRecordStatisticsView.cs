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

		public PlayerPersonalRecordStatisticsView(int mostHeadshots, int mostNutshots, int mostConsecutiveSnipes, int mostXPEarned, int mostSplats, int mostDamageDealt, int mostDamageReceived, int mostArmorPickedUp, int mostHealthPickedUp, int mostMeleeSplats, int mostMachinegunSplats, int mostShotgunSplats, int mostSniperSplats, int mostSplattergunSplats, int mostCannonSplats, int mostLauncherSplats)
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
			var builder = new StringBuilder();
			builder.Append("[PlayerPersonalRecordStatisticsView: ");
			builder.Append("[MostArmorPickedUp: ");
			builder.Append(MostArmorPickedUp);
			builder.Append("][MostCannonSplats: ");
			builder.Append(MostCannonSplats);
			builder.Append("][MostConsecutiveSnipes: ");
			builder.Append(MostConsecutiveSnipes);
			builder.Append("][MostDamageDealt: ");
			builder.Append(MostDamageDealt);
			builder.Append("][MostDamageReceived: ");
			builder.Append(MostDamageReceived);
			builder.Append("][MostHeadshots: ");
			builder.Append(MostHeadshots);
			builder.Append("][MostHealthPickedUp: ");
			builder.Append(MostHealthPickedUp);
			builder.Append("][MostLauncherSplats: ");
			builder.Append(MostLauncherSplats);
			builder.Append("][MostMachinegunSplats: ");
			builder.Append(MostMachinegunSplats);
			builder.Append("][MostMeleeSplats: ");
			builder.Append(MostMeleeSplats);
			builder.Append("][MostNutshots: ");
			builder.Append(MostNutshots);
			builder.Append("][MostShotgunSplats: ");
			builder.Append(MostShotgunSplats);
			builder.Append("][MostSniperSplats: ");
			builder.Append(MostSniperSplats);
			builder.Append("][MostSplats: ");
			builder.Append(MostSplats);
			builder.Append("][MostSplattergunSplats: ");
			builder.Append(MostSplattergunSplats);
			builder.Append("][MostXPEarned: ");
			builder.Append(MostXPEarned);
			builder.Append("]]");
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
