using System;
using System.Text;

namespace UberStrok.Core.Views
{
	[Serializable]
	public class PlayerWeaponStatisticsView
	{
		public PlayerWeaponStatisticsView()
		{
            // Space
		}

		public PlayerWeaponStatisticsView(int meleeTotalSplats, int machineGunTotalSplats, int shotgunTotalSplats, int sniperTotalSplats, int splattergunTotalSplats, int cannonTotalSplats, int launcherTotalSplats, int meleeTotalShotsFired, int meleeTotalShotsHit, int meleeTotalDamageDone, int machineGunTotalShotsFired, int machineGunTotalShotsHit, int machineGunTotalDamageDone, int shotgunTotalShotsFired, int shotgunTotalShotsHit, int shotgunTotalDamageDone, int sniperTotalShotsFired, int sniperTotalShotsHit, int sniperTotalDamageDone, int splattergunTotalShotsFired, int splattergunTotalShotsHit, int splattergunTotalDamageDone, int cannonTotalShotsFired, int cannonTotalShotsHit, int cannonTotalDamageDone, int launcherTotalShotsFired, int launcherTotalShotsHit, int launcherTotalDamageDone)
		{
			CannonTotalDamageDone = cannonTotalDamageDone;
			CannonTotalShotsFired = cannonTotalShotsFired;
			CannonTotalShotsHit = cannonTotalShotsHit;
			CannonTotalSplats = cannonTotalSplats;
			LauncherTotalDamageDone = launcherTotalDamageDone;
			LauncherTotalShotsFired = launcherTotalShotsFired;
			LauncherTotalShotsHit = launcherTotalShotsHit;
			LauncherTotalSplats = launcherTotalSplats;
			MachineGunTotalDamageDone = machineGunTotalDamageDone;
			MachineGunTotalShotsFired = machineGunTotalShotsFired;
			MachineGunTotalShotsHit = machineGunTotalShotsHit;
			MachineGunTotalSplats = machineGunTotalSplats;
			MeleeTotalDamageDone = meleeTotalDamageDone;
			MeleeTotalShotsFired = meleeTotalShotsFired;
			MeleeTotalShotsHit = meleeTotalShotsHit;
			MeleeTotalSplats = meleeTotalSplats;
			ShotgunTotalDamageDone = shotgunTotalDamageDone;
			ShotgunTotalShotsFired = shotgunTotalShotsFired;
			ShotgunTotalShotsHit = shotgunTotalShotsHit;
			ShotgunTotalSplats = shotgunTotalSplats;
			SniperTotalDamageDone = sniperTotalDamageDone;
			SniperTotalShotsFired = sniperTotalShotsFired;
			SniperTotalShotsHit = sniperTotalShotsHit;
			SniperTotalSplats = sniperTotalSplats;
			SplattergunTotalDamageDone = splattergunTotalDamageDone;
			SplattergunTotalShotsFired = splattergunTotalShotsFired;
			SplattergunTotalShotsHit = splattergunTotalShotsHit;
			SplattergunTotalSplats = splattergunTotalSplats;
		}

		public override string ToString()
		{
			var builder = new StringBuilder();
			builder.Append("[PlayerWeaponStatisticsView: ");
			builder.Append("[CannonTotalDamageDone: ");
			builder.Append(CannonTotalDamageDone);
			builder.Append("][CannonTotalShotsFired: ");
			builder.Append(CannonTotalShotsFired);
			builder.Append("][CannonTotalShotsHit: ");
			builder.Append(CannonTotalShotsHit);
			builder.Append("][CannonTotalSplats: ");
			builder.Append(CannonTotalSplats);
			builder.Append("][LauncherTotalDamageDone: ");
			builder.Append(LauncherTotalDamageDone);
			builder.Append("][LauncherTotalShotsFired: ");
			builder.Append(LauncherTotalShotsFired);
			builder.Append("][LauncherTotalShotsHit: ");
			builder.Append(LauncherTotalShotsHit);
			builder.Append("][LauncherTotalSplats: ");
			builder.Append(LauncherTotalSplats);
			builder.Append("][MachineGunTotalDamageDone: ");
			builder.Append(MachineGunTotalDamageDone);
			builder.Append("][MachineGunTotalShotsFired: ");
			builder.Append(MachineGunTotalShotsFired);
			builder.Append("][MachineGunTotalShotsHit: ");
			builder.Append(MachineGunTotalShotsHit);
			builder.Append("][MachineGunTotalSplats: ");
			builder.Append(MachineGunTotalSplats);
			builder.Append("][MeleeTotalDamageDone: ");
			builder.Append(MeleeTotalDamageDone);
			builder.Append("][MeleeTotalShotsFired: ");
			builder.Append(MeleeTotalShotsFired);
			builder.Append("][MeleeTotalShotsHit: ");
			builder.Append(MeleeTotalShotsHit);
			builder.Append("][MeleeTotalSplats: ");
			builder.Append(MeleeTotalSplats);
			builder.Append("][ShotgunTotalDamageDone: ");
			builder.Append(ShotgunTotalDamageDone);
			builder.Append("][ShotgunTotalShotsFired: ");
			builder.Append(ShotgunTotalShotsFired);
			builder.Append("][ShotgunTotalShotsHit: ");
			builder.Append(ShotgunTotalShotsHit);
			builder.Append("][ShotgunTotalSplats: ");
			builder.Append(ShotgunTotalSplats);
			builder.Append("][SniperTotalDamageDone: ");
			builder.Append(SniperTotalDamageDone);
			builder.Append("][SniperTotalShotsFired: ");
			builder.Append(SniperTotalShotsFired);
			builder.Append("][SniperTotalShotsHit: ");
			builder.Append(SniperTotalShotsHit);
			builder.Append("][SniperTotalSplats: ");
			builder.Append(SniperTotalSplats);
			builder.Append("][SplattergunTotalDamageDone: ");
			builder.Append(SplattergunTotalDamageDone);
			builder.Append("][SplattergunTotalShotsFired: ");
			builder.Append(SplattergunTotalShotsFired);
			builder.Append("][SplattergunTotalShotsHit: ");
			builder.Append(SplattergunTotalShotsHit);
			builder.Append("][SplattergunTotalSplats: ");
			builder.Append(SplattergunTotalSplats);
			builder.Append("]]");
			return builder.ToString();
		}

		public int CannonTotalDamageDone { get; set; }
		public int CannonTotalShotsFired { get; set; }
		public int CannonTotalShotsHit { get; set; }
		public int CannonTotalSplats { get; set; }
		public int LauncherTotalDamageDone { get; set; }
		public int LauncherTotalShotsFired { get; set; }
		public int LauncherTotalShotsHit { get; set; }
		public int LauncherTotalSplats { get; set; }
		public int MachineGunTotalDamageDone { get; set; }
		public int MachineGunTotalShotsFired { get; set; }
		public int MachineGunTotalShotsHit { get; set; }
		public int MachineGunTotalSplats { get; set; }
		public int MeleeTotalDamageDone { get; set; }
		public int MeleeTotalShotsFired { get; set; }
		public int MeleeTotalShotsHit { get; set; }
		public int MeleeTotalSplats { get; set; }
		public int ShotgunTotalDamageDone { get; set; }
		public int ShotgunTotalShotsFired { get; set; }
		public int ShotgunTotalShotsHit { get; set; }
		public int ShotgunTotalSplats { get; set; }
		public int SniperTotalDamageDone { get; set; }
		public int SniperTotalShotsFired { get; set; }
		public int SniperTotalShotsHit { get; set; }
		public int SniperTotalSplats { get; set; }
		public int SplattergunTotalDamageDone { get; set; }
		public int SplattergunTotalShotsFired { get; set; }
		public int SplattergunTotalShotsHit { get; set; }
		public int SplattergunTotalSplats { get; set; }
	}
}
