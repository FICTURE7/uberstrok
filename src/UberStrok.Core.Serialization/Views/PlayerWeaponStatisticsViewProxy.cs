using System.IO;
using UberStrok.Core.Views;

namespace UberStrok.Core.Serialization.Views
{
    public static class PlayerWeaponStatisticsViewProxy
	{
		public static PlayerWeaponStatisticsView Deserialize(Stream bytes)
		{
			return new PlayerWeaponStatisticsView
			{
				CannonTotalDamageDone = Int32Proxy.Deserialize(bytes),
				CannonTotalShotsFired = Int32Proxy.Deserialize(bytes),
				CannonTotalShotsHit = Int32Proxy.Deserialize(bytes),
				CannonTotalSplats = Int32Proxy.Deserialize(bytes),
				LauncherTotalDamageDone = Int32Proxy.Deserialize(bytes),
				LauncherTotalShotsFired = Int32Proxy.Deserialize(bytes),
				LauncherTotalShotsHit = Int32Proxy.Deserialize(bytes),
				LauncherTotalSplats = Int32Proxy.Deserialize(bytes),
				MachineGunTotalDamageDone = Int32Proxy.Deserialize(bytes),
				MachineGunTotalShotsFired = Int32Proxy.Deserialize(bytes),
				MachineGunTotalShotsHit = Int32Proxy.Deserialize(bytes),
				MachineGunTotalSplats = Int32Proxy.Deserialize(bytes),
				MeleeTotalDamageDone = Int32Proxy.Deserialize(bytes),
				MeleeTotalShotsFired = Int32Proxy.Deserialize(bytes),
				MeleeTotalShotsHit = Int32Proxy.Deserialize(bytes),
				MeleeTotalSplats = Int32Proxy.Deserialize(bytes),
				ShotgunTotalDamageDone = Int32Proxy.Deserialize(bytes),
				ShotgunTotalShotsFired = Int32Proxy.Deserialize(bytes),
				ShotgunTotalShotsHit = Int32Proxy.Deserialize(bytes),
				ShotgunTotalSplats = Int32Proxy.Deserialize(bytes),
				SniperTotalDamageDone = Int32Proxy.Deserialize(bytes),
				SniperTotalShotsFired = Int32Proxy.Deserialize(bytes),
				SniperTotalShotsHit = Int32Proxy.Deserialize(bytes),
				SniperTotalSplats = Int32Proxy.Deserialize(bytes),
				SplattergunTotalDamageDone = Int32Proxy.Deserialize(bytes),
				SplattergunTotalShotsFired = Int32Proxy.Deserialize(bytes),
				SplattergunTotalShotsHit = Int32Proxy.Deserialize(bytes),
				SplattergunTotalSplats = Int32Proxy.Deserialize(bytes)
			};
		}

		public static void Serialize(Stream stream, PlayerWeaponStatisticsView instance)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Int32Proxy.Serialize(memoryStream, instance.CannonTotalDamageDone);
				Int32Proxy.Serialize(memoryStream, instance.CannonTotalShotsFired);
				Int32Proxy.Serialize(memoryStream, instance.CannonTotalShotsHit);
				Int32Proxy.Serialize(memoryStream, instance.CannonTotalSplats);
				Int32Proxy.Serialize(memoryStream, instance.LauncherTotalDamageDone);
				Int32Proxy.Serialize(memoryStream, instance.LauncherTotalShotsFired);
				Int32Proxy.Serialize(memoryStream, instance.LauncherTotalShotsHit);
				Int32Proxy.Serialize(memoryStream, instance.LauncherTotalSplats);
				Int32Proxy.Serialize(memoryStream, instance.MachineGunTotalDamageDone);
				Int32Proxy.Serialize(memoryStream, instance.MachineGunTotalShotsFired);
				Int32Proxy.Serialize(memoryStream, instance.MachineGunTotalShotsHit);
				Int32Proxy.Serialize(memoryStream, instance.MachineGunTotalSplats);
				Int32Proxy.Serialize(memoryStream, instance.MeleeTotalDamageDone);
				Int32Proxy.Serialize(memoryStream, instance.MeleeTotalShotsFired);
				Int32Proxy.Serialize(memoryStream, instance.MeleeTotalShotsHit);
				Int32Proxy.Serialize(memoryStream, instance.MeleeTotalSplats);
				Int32Proxy.Serialize(memoryStream, instance.ShotgunTotalDamageDone);
				Int32Proxy.Serialize(memoryStream, instance.ShotgunTotalShotsFired);
				Int32Proxy.Serialize(memoryStream, instance.ShotgunTotalShotsHit);
				Int32Proxy.Serialize(memoryStream, instance.ShotgunTotalSplats);
				Int32Proxy.Serialize(memoryStream, instance.SniperTotalDamageDone);
				Int32Proxy.Serialize(memoryStream, instance.SniperTotalShotsFired);
				Int32Proxy.Serialize(memoryStream, instance.SniperTotalShotsHit);
				Int32Proxy.Serialize(memoryStream, instance.SniperTotalSplats);
				Int32Proxy.Serialize(memoryStream, instance.SplattergunTotalDamageDone);
				Int32Proxy.Serialize(memoryStream, instance.SplattergunTotalShotsFired);
				Int32Proxy.Serialize(memoryStream, instance.SplattergunTotalShotsHit);
				Int32Proxy.Serialize(memoryStream, instance.SplattergunTotalSplats);
				memoryStream.WriteTo(stream);
			}
		}
	}
}
