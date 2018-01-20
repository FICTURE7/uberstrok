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

#if false
        public int AccuracySpread
		{
			get
			{
				return this._accuracySpread;
			}
			set
			{
				this._accuracySpread = value;
			}
		}

		public int CombatRange
		{
			get
			{
				return this._combatRange;
			}
			set
			{
				this._combatRange = value;
			}
		}

		public int CriticalStrikeBonus
		{
			get
			{
				return this._criticalStrikeBonus;
			}
			set
			{
				this._criticalStrikeBonus = value;
			}
		}

		public int DamageKnockback
		{
			get
			{
				return this._damageKnockback;
			}
			set
			{
				this._damageKnockback = value;
			}
		}

		public int DamagePerProjectile
		{
			get
			{
				return this._damagePerProjectile;
			}
			set
			{
				this._damagePerProjectile = value;
			}
		}

		public float DamagePerSecond
		{
			get
			{
				return (float)((this.RateOfFire == 0) ? 0 : (this.DamagePerProjectile * this.ProjectilesPerShot / this.RateOfFire));
			}
		}

		public int DefaultZoomMultiplier
		{
			get
			{
				return this._defaultZoomMultiplier;
			}
			set
			{
				this._defaultZoomMultiplier = value;
			}
		}

		public bool HasAutomaticFire
		{
			get
			{
				return this._hasAutoFire;
			}
			set
			{
				this._hasAutoFire = value;
			}
		}

		public override UberstrikeItemType ItemType
		{
			get
			{
				return UberstrikeItemType.Weapon;
			}
		}

		public int MaxAmmo
		{
			get
			{
				return this._maxAmmo;
			}
			set
			{
				this._maxAmmo = value;
			}
		}

		public int MaxZoomMultiplier
		{
			get
			{
				return this._maxZoomMultiplier;
			}
			set
			{
				this._maxZoomMultiplier = value;
			}
		}

		public int MinZoomMultiplier
		{
			get
			{
				return this._minZoomMultiplier;
			}
			set
			{
				this._minZoomMultiplier = value;
			}
		}

		public int MissileBounciness
		{
			get
			{
				return this._missileBounciness;
			}
			set
			{
				this._missileBounciness = value;
			}
		}

		public int MissileForceImpulse
		{
			get
			{
				return this._missileForceImpulse;
			}
			set
			{
				this._missileForceImpulse = value;
			}
		}

		public int MissileTimeToDetonate
		{
			get
			{
				return this._missileTimeToDetonate;
			}
			set
			{
				this._missileTimeToDetonate = value;
			}
		}

		public int ProjectileSpeed
		{
			get
			{
				return this._projectileSpeed;
			}
			set
			{
				this._projectileSpeed = value;
			}
		}

		public int ProjectilesPerShot
		{
			get
			{
				return this._projectilesPerShot;
			}
			set
			{
				this._projectilesPerShot = value;
			}
		}

		public int RateOfFire
		{
			get
			{
				return this._rateOfFire;
			}
			set
			{
				this._rateOfFire = value;
			}
		}

		public int RecoilKickback
		{
			get
			{
				return this._recoilKickback;
			}
			set
			{
				this._recoilKickback = value;
			}
		}

		public int RecoilMovement
		{
			get
			{
				return this._recoilMovement;
			}
			set
			{
				this._recoilMovement = value;
			}
		}

		public int SecondaryActionReticle
		{
			get
			{
				return this._secondaryActionReticle;
			}
			set
			{
				this._secondaryActionReticle = value;
			}
		}

		public int SplashRadius
		{
			get
			{
				return this._splashRadius;
			}
			set
			{
				this._splashRadius = value;
			}
		}

		public int StartAmmo
		{
			get
			{
				return this._startAmmo;
			}
			set
			{
				this._startAmmo = value;
			}
		}

		public int Tier
		{
			get
			{
				return this._tier;
			}
			set
			{
				this._tier = value;
			}
		}

		public int WeaponSecondaryAction
		{
			get
			{
				return this._weaponSecondaryAction;
			}
			set
			{
				this._weaponSecondaryAction = value;
			}
		}

		[SerializeField]
		private int _accuracySpread;

		[SerializeField]
		private int _combatRange;

		private int _criticalStrikeBonus;

		[SerializeField]
		private int _damageKnockback;

		[SerializeField]
		private int _damagePerProjectile;

		[SerializeField]
		private int _defaultZoomMultiplier;

		[SerializeField]
		private bool _hasAutoFire;

		[SerializeField]
		private int _maxAmmo;

		[SerializeField]
		private int _maxZoomMultiplier;

		[SerializeField]
		private int _minZoomMultiplier;

		[SerializeField]
		private int _missileBounciness;

		[SerializeField]
		private int _missileForceImpulse;

		[SerializeField]
		private int _missileTimeToDetonate;

		[SerializeField]
		private int _projectileSpeed;

		[SerializeField]
		private int _projectilesPerShot;

		[SerializeField]
		private int _rateOfFire;

		[SerializeField]
		private int _recoilKickback;

		[SerializeField]
		private int _recoilMovement;

		[SerializeField]
		private int _secondaryActionReticle;

		[SerializeField]
		private int _splashRadius;

		[SerializeField]
		private int _startAmmo;

		[SerializeField]
		private int _tier;

		[SerializeField]
		private int _weaponSecondaryAction;
#endif
    }
}
