using System.Collections.Generic;
using System.Reflection;
using UberStrok.Core.Common;
using UberStrok.Core.Views;

namespace UberStrok.Core
{
    public class StatisticsManager
    {
        private UberStrikeItemClass _lastKillClass;
        private readonly static Dictionary<string, PropertyInfo> _properties;

        static StatisticsManager()
        {
            _properties = new Dictionary<string, PropertyInfo>();
        }

        public int MostConsecutiveKills { get; set; }
        public int CurrentConsecutiveKills { get; set; }

        public StatsCollectionView Total { get; private set; }
        public StatsCollectionView Best { get; private set; }
        public StatsCollectionView Current { get; private set; }

        public StatisticsManager()
        {
            Total = new StatsCollectionView();
            Best = new StatsCollectionView();

            Reset(hard: false);
        }

        public void RecordHeadshot(int count = 1)
        {
            RecordStats(nameof(StatsCollectionView.Headshots), count);
        }

        public void RecordNutshot(int count = 1)
        {
            RecordStats(nameof(StatsCollectionView.Nutshots), count);
        }

        public void RecordSuicide(int count = 1)
        {
            RecordStats(nameof(StatsCollectionView.Suicides), count);
        }

        public void RecordDeath(int count = 1)
        {
            RecordStats(nameof(StatsCollectionView.Deaths), count);
        }

        public void RecordHealthPickedUp(int count = 1)
        {
            RecordStats(nameof(StatsCollectionView.HealthPickedUp), count);
        }

        public void RecordArmorPickedUp(int count = 1)
        {
            RecordStats(nameof(StatsCollectionView.ArmorPickedUp), count);
        }

        public void RecordDamageReceived(int count)
        {
            RecordStats(nameof(StatsCollectionView.DamageReceived), count);
        }

        public void RecordKill(UberStrikeItemClass itemClass, int count = 1)
        {
            if ((CurrentConsecutiveKills += count) > MostConsecutiveKills)
                MostConsecutiveKills = CurrentConsecutiveKills;

            switch (itemClass)
            {
                case UberStrikeItemClass.WeaponMelee:
                    RecordStats(nameof(StatsCollectionView.MeleeKills), count);
                    break;
                case UberStrikeItemClass.WeaponMachinegun:
                    RecordStats(nameof(StatsCollectionView.MachineGunKills), count);
                    break;
                case UberStrikeItemClass.WeaponShotgun:
                    RecordStats(nameof(StatsCollectionView.ShotgunSplats), count);
                    break;
                case UberStrikeItemClass.WeaponSniperRifle:
                    RecordStats(nameof(StatsCollectionView.SniperKills), count);
                    if (_lastKillClass == UberStrikeItemClass.WeaponSniperRifle)
                        RecordStats(nameof(StatsCollectionView.ConsecutiveSnipes), count);
                    break;
                case UberStrikeItemClass.WeaponSplattergun:
                    RecordStats(nameof(StatsCollectionView.SplattergunKills), count);
                    break;
                case UberStrikeItemClass.WeaponCannon:
                    RecordStats(nameof(StatsCollectionView.CannonKills), count);
                    break;
                case UberStrikeItemClass.WeaponLauncher:
                    RecordStats(nameof(StatsCollectionView.LauncherKills), count);
                    break;

                default:
                    /* Skip assignment to _lastKillClass. */
                    Current.ConsecutiveSnipes = 0;
                    return;
            }

            if (itemClass != UberStrikeItemClass.WeaponSniperRifle)
                Current.ConsecutiveSnipes = 0;

            _lastKillClass = itemClass;
        }

        public void RecordHit(UberStrikeItemClass itemClass, int count = 1)
        {
            switch (itemClass)
            {
                case UberStrikeItemClass.WeaponMelee:
                    RecordStats(nameof(StatsCollectionView.MeleeShotsHit), count);
                    break;
                case UberStrikeItemClass.WeaponMachinegun:
                    RecordStats(nameof(StatsCollectionView.MachineGunShotsHit), count);
                    break;
                case UberStrikeItemClass.WeaponShotgun:
                    RecordStats(nameof(StatsCollectionView.ShotgunShotsHit), count);
                    break;
                case UberStrikeItemClass.WeaponSniperRifle:
                    RecordStats(nameof(StatsCollectionView.SniperShotsHit), count);
                    break;
                case UberStrikeItemClass.WeaponSplattergun:
                    RecordStats(nameof(StatsCollectionView.SplattergunShotsHit), count);
                    break;
                case UberStrikeItemClass.WeaponCannon:
                    RecordStats(nameof(StatsCollectionView.CannonShotsHit), count);
                    break;
                case UberStrikeItemClass.WeaponLauncher:
                    RecordStats(nameof(StatsCollectionView.LauncherShotsHit), count);
                    break;
            }
        }

        public void RecordShot(UberStrikeItemClass itemClass, int count)
        {
            switch (itemClass)
            {
                case UberStrikeItemClass.WeaponMelee:
                    RecordStats(nameof(StatsCollectionView.MeleeShotsFired), count);
                    break;
                case UberStrikeItemClass.WeaponMachinegun:
                    RecordStats(nameof(StatsCollectionView.MachineGunShotsFired), count);
                    break;
                case UberStrikeItemClass.WeaponShotgun:
                    RecordStats(nameof(StatsCollectionView.ShotgunShotsFired), count);
                    break;
                case UberStrikeItemClass.WeaponSniperRifle:
                    RecordStats(nameof(StatsCollectionView.SniperShotsFired), count);
                    break;
                case UberStrikeItemClass.WeaponSplattergun:
                    RecordStats(nameof(StatsCollectionView.SplattergunShotsFired), count);
                    break;
                case UberStrikeItemClass.WeaponCannon:
                    RecordStats(nameof(StatsCollectionView.CannonShotsFired), count);
                    break;
                case UberStrikeItemClass.WeaponLauncher:
                    RecordStats(nameof(StatsCollectionView.LauncherShotsFired), count);
                    break;
            }
        }

        public void RecordDamageDealt(UberStrikeItemClass itemClass, int count)
        {
            switch (itemClass)
            {
                case UberStrikeItemClass.WeaponMelee:
                    RecordStats(nameof(StatsCollectionView.MeleeDamageDone), count);
                    break;
                case UberStrikeItemClass.WeaponMachinegun:
                    RecordStats(nameof(StatsCollectionView.MachineGunDamageDone), count);
                    break;
                case UberStrikeItemClass.WeaponShotgun:
                    RecordStats(nameof(StatsCollectionView.ShotgunDamageDone), count);
                    break;
                case UberStrikeItemClass.WeaponSniperRifle:
                    RecordStats(nameof(StatsCollectionView.SniperDamageDone), count);
                    break;
                case UberStrikeItemClass.WeaponSplattergun:
                    RecordStats(nameof(StatsCollectionView.SplattergunDamageDone), count);
                    break;
                case UberStrikeItemClass.WeaponCannon:
                    RecordStats(nameof(StatsCollectionView.CannonDamageDone), count);
                    break;
                case UberStrikeItemClass.WeaponLauncher:
                    RecordStats(nameof(StatsCollectionView.LauncherDamageDone), count);
                    break;
            }
        }

        public void Reset(bool hard)
        {
            if (hard)
            {
                Total = new StatsCollectionView();
                Best = new StatsCollectionView();
                MostConsecutiveKills = 0;
            }

            Current = new StatsCollectionView();
            CurrentConsecutiveKills = 0;
        }

        private void RecordStats(string propertyName, int count)
        {
            if (!_properties.TryGetValue(propertyName, out PropertyInfo statsProperty))
            {
                statsProperty = typeof(StatsCollectionView).GetProperty(propertyName);
                _properties.Add(propertyName, statsProperty);
            }

            /* 
             * Total.property += count; 
             * Current.property += count;
             * 
             * If (Current.property > Best.property)
             *      Best.property = Current.property;
             */
            AddStats(Total, statsProperty, count);

            int current = AddStats(Current, statsProperty, count);
            if (current > GetStats(Best, statsProperty))
                SetStats(Best, statsProperty, current);
        }

        private void SetStats(StatsCollectionView stats, string propertyName, int value)
        {
            if (!_properties.TryGetValue(propertyName, out PropertyInfo statsProperty))
            {
                statsProperty = typeof(StatsCollectionView).GetProperty(propertyName);
                _properties.Add(propertyName, statsProperty);
            }

            SetStats(stats, statsProperty, value);
        }

        private int GetStats(StatsCollectionView stats, PropertyInfo property)
        {
            /* return stats.property; */
            return (int)property.GetMethod.Invoke(stats, null);
        }

        private void SetStats(StatsCollectionView stats, PropertyInfo property, int value)
        {
            /* stats.property = value; */
            property.SetMethod.Invoke(stats, new object[] { value });
        }

        private int AddStats(StatsCollectionView stats, PropertyInfo property, int count)
        {
            /* stats.property += count; */
            int newValue = GetStats(stats, property) + count;
            SetStats(stats, property, newValue);
            return newValue;
        }
    }
}
