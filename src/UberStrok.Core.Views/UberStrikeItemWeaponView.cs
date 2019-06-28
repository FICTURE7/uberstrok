using System;
using UberStrok.Core.Common;

namespace UberStrok.Core.Views
{
    [Serializable]
    public class UberStrikeItemWeaponView : BaseUberStrikeItemView
    {
        public int AccuracySpread { get; set; }
        public int CombatRange { get; set; }
        public int CriticalStrikeBonus { get; set; }
        public int DamageKnockback { get; set; }
        public int DamagePerProjectile { get; set; }
        public float DamagePerSecond => RateOfFire == 0 ? 0 : (DamagePerProjectile * ProjectilesPerShot / RateOfFire);
        public int DefaultZoomMultiplier { get; set; }
        public bool HasAutomaticFire { get; set; }
        public int MaxAmmo { get; set; }
        public int MaxZoomMultiplier { get; set; }
        public int MinZoomMultiplier { get; set; }
        public int MissileBounciness { get; set; }
        public int MissileForceImpulse { get; set; }
        public int MissileTimeToDetonate { get; set; }
        public int ProjectileSpeed { get; set; }
        public int ProjectilesPerShot { get; set; }
        public int RateOfFire { get; set; }
        public int RecoilKickback { get; set; }
        public int RecoilMovement { get; set; }
        public int SecondaryActionReticle { get; set; }
        public int SplashRadius { get; set; }
        public int StartAmmo { get; set; }
        public int Tier { get; set; }
        public int WeaponSecondaryAction { get; set; }

        public override UberStrikeItemType ItemType => UberStrikeItemType.Weapon;
    }
}
