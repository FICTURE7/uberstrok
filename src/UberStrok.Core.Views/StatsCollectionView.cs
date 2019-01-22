using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UberStrok.Core.Views
{
    [Serializable]
    public class StatsCollectionView
    {

        public int GetKills()
        {
            return this.MeleeKills + this.MachineGunKills + this.ShotgunSplats + this.SniperKills + this.SplattergunKills + this.CannonKills + this.LauncherKills - this.Suicides;
        }

        public int GetShots()
        {
            return this.MeleeShotsFired + this.MachineGunShotsFired + this.ShotgunShotsFired + this.SniperShotsFired + this.SplattergunShotsFired + this.CannonShotsFired + this.LauncherShotsFired;
        }

        public int GetHits()
        {
            return this.MeleeShotsHit + this.MachineGunShotsHit + this.ShotgunShotsHit + this.SniperShotsHit + this.SplattergunShotsHit + this.CannonShotsHit + this.LauncherShotsHit;
        }

        public int GetDamageDealt()
        {
            return this.MeleeDamageDone + this.MachineGunDamageDone + this.ShotgunDamageDone + this.SniperDamageDone + this.SplattergunDamageDone + this.CannonDamageDone + this.LauncherDamageDone;
        }

        
        public int Xp { get; set; }
        public int DamageReceived { get; set; }

        /* COMPLETED METRICS BEYOND THIS POINT */

        public int Headshots { get; set; }
        public int Nutshots { get; set; }
        public int ConsecutiveSnipes { get; set; }
        public int Deaths { get; set; }
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
        public int Suicides { get; set; }

        /* END OF POINT */

        public int Points { get; set; }
    }
}