using System.IO;
using UberStrok.Core.Views;

namespace UberStrok.Core.Serialization.Views
{
    public static class PlayerPersonalRecordStatisticsViewProxy
	{
		public static PlayerPersonalRecordStatisticsView Deserialize(Stream bytes)
		{
			return new PlayerPersonalRecordStatisticsView
			{
				MostArmorPickedUp = Int32Proxy.Deserialize(bytes),
				MostCannonSplats = Int32Proxy.Deserialize(bytes),
				MostConsecutiveSnipes = Int32Proxy.Deserialize(bytes),
				MostDamageDealt = Int32Proxy.Deserialize(bytes),
				MostDamageReceived = Int32Proxy.Deserialize(bytes),
				MostHeadshots = Int32Proxy.Deserialize(bytes),
				MostHealthPickedUp = Int32Proxy.Deserialize(bytes),
				MostLauncherSplats = Int32Proxy.Deserialize(bytes),
				MostMachinegunSplats = Int32Proxy.Deserialize(bytes),
				MostMeleeSplats = Int32Proxy.Deserialize(bytes),
				MostNutshots = Int32Proxy.Deserialize(bytes),
				MostShotgunSplats = Int32Proxy.Deserialize(bytes),
				MostSniperSplats = Int32Proxy.Deserialize(bytes),
				MostSplats = Int32Proxy.Deserialize(bytes),
				MostSplattergunSplats = Int32Proxy.Deserialize(bytes),
				MostXPEarned = Int32Proxy.Deserialize(bytes)
			};
		}

		public static void Serialize(Stream stream, PlayerPersonalRecordStatisticsView instance)
		{
			using (var bytes = new MemoryStream())
			{
				Int32Proxy.Serialize(bytes, instance.MostArmorPickedUp);
				Int32Proxy.Serialize(bytes, instance.MostCannonSplats);
				Int32Proxy.Serialize(bytes, instance.MostConsecutiveSnipes);
				Int32Proxy.Serialize(bytes, instance.MostDamageDealt);
				Int32Proxy.Serialize(bytes, instance.MostDamageReceived);
				Int32Proxy.Serialize(bytes, instance.MostHeadshots);
				Int32Proxy.Serialize(bytes, instance.MostHealthPickedUp);
				Int32Proxy.Serialize(bytes, instance.MostLauncherSplats);
				Int32Proxy.Serialize(bytes, instance.MostMachinegunSplats);
				Int32Proxy.Serialize(bytes, instance.MostMeleeSplats);
				Int32Proxy.Serialize(bytes, instance.MostNutshots);
				Int32Proxy.Serialize(bytes, instance.MostShotgunSplats);
				Int32Proxy.Serialize(bytes, instance.MostSniperSplats);
				Int32Proxy.Serialize(bytes, instance.MostSplats);
				Int32Proxy.Serialize(bytes, instance.MostSplattergunSplats);
				Int32Proxy.Serialize(bytes, instance.MostXPEarned);
				bytes.WriteTo(stream);
			}
		}
	}
}
