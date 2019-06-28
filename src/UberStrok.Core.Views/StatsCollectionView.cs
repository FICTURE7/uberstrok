using System;
using UberStrok.Core.Common;

namespace UberStrok.Core.Views
{
    [Serializable]
    public class StatsCollectionView
    {
        public int Headshots { get; set; }
        public int Nutshots { get; set; }
        public int DamageReceived { get; set; }
        public int Xp { get; set; }
        public int Deaths { get; set; }
        public int Suicides { get; set; }
        public int Points { get; set; }

        public int ConsecutiveSnipes { get; set; }

        public int ArmorPickedUp { get; set; }
        public int HealthPickedUp { get; set; }
        
        public int MeleeKills { get; set; }
        public int MeleeShotsFired { get; set; }
        public int MeleeShotsHit { get; set; }
        public int MeleeDamageDone { get; set; }

        public int MachineGunKills { get; set; }
        public int MachineGunShotsFired { get; set; }
        public int MachineGunShotsHit { get; set; }
        public int MachineGunDamageDone { get; set; }

        public int ShotgunSplats { get; set; }
        public int ShotgunShotsFired { get; set; }
        public int ShotgunShotsHit { get; set; }
        public int ShotgunDamageDone { get; set; }

        public int SniperKills { get; set; }
        public int SniperShotsFired { get; set; }
        public int SniperShotsHit { get; set; }
        public int SniperDamageDone { get; set; }

        public int SplattergunKills { get; set; }
        public int SplattergunShotsFired { get; set; }
        public int SplattergunShotsHit { get; set; }
        public int SplattergunDamageDone { get; set; }

        public int CannonKills { get; set; }
        public int CannonShotsFired { get; set; }
        public int CannonShotsHit { get; set; }
        public int CannonDamageDone { get; set; }

        public int LauncherKills { get; set; }
        public int LauncherShotsFired { get; set; }
        public int LauncherShotsHit { get; set; }
        public int LauncherDamageDone { get; set; }

        public int GetKills()
        {
            return MeleeKills + MachineGunKills + ShotgunSplats + SniperKills +
                SplattergunKills + CannonKills + LauncherKills - Suicides;
        }

        public int GetShots()
        {
            return MeleeShotsFired + MachineGunShotsFired + ShotgunShotsFired + SniperShotsFired +
                SplattergunShotsFired + CannonShotsFired + LauncherShotsFired;
        }

        public int GetHits()
        {
            return MeleeShotsHit + MachineGunShotsHit + ShotgunShotsHit + SniperShotsHit +
                SplattergunShotsHit + CannonShotsHit + LauncherShotsHit;
        }

        public int GetDamageDealt()
        {
            return MeleeDamageDone + MachineGunDamageDone + ShotgunDamageDone + SniperDamageDone +
                SplattergunDamageDone + CannonDamageDone + LauncherDamageDone;
        }

        public float GetAccuracy()
        {
            var shots = GetShots();
            if (shots == 0)
                return 0f;
            
            return GetHits() / shots;
        }
    }
}