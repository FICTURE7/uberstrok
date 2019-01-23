using Photon.SocketServer;
using System;
using System.Linq;
using UberStrok.Core.Common;
using System.Collections.Generic;
using UberStrok.Core;
using UberStrok.Core.Views;
using MoreLinq;

namespace UberStrok.Realtime.Server.Game
{
    public class GamePeer : BasePeer
    {
        private readonly GamePeerEvents _events;
        private readonly StateMachine<PeerState.Id> _state;

        public GamePeer(InitRequest initRequest) : base(initRequest)
        {
            _events = new GamePeerEvents(this);

            _state = new StateMachine<PeerState.Id>();
            _state.Register(PeerState.Id.None, null);
            _state.Register(PeerState.Id.Overview, new OverviewPeerState(this));
            _state.Register(PeerState.Id.WaitingForPlayers, new WaitingForPlayersPeerState(this));
            _state.Register(PeerState.Id.Countdown, new CountdownPeerState(this));
            _state.Register(PeerState.Id.Playing, new PlayingPeerState(this));
            _state.Register(PeerState.Id.Killed, new KilledPeerState(this));

            KnownActors = new List<int>(16);
            /* Could make GamePeerOperationHandler a singleton but what ever. */
            AddOperationHandler(new GamePeerOperationHandler());
        }

        public void IncrementShotsFired(UberStrikeItemClass itemClass, int weaponId)
        {
            if (WeaponStats.ContainsKey(weaponId))
                WeaponStats[weaponId].ShotsFired++;
            else
            {
                WeaponStats.Add(weaponId, new WeaponStats());
                WeaponStats[weaponId].ShotsFired++;
                WeaponStats[weaponId].ItemClass = itemClass;
            }
            switch (itemClass)
            {
                case UberStrikeItemClass.WeaponShotgun:
                    TotalStats.ShotgunShotsFired++;
                    CurrentLifeStats.ShotgunShotsFired++;
                    break;
                case UberStrikeItemClass.WeaponSniperRifle:
                    TotalStats.SniperShotsFired++;
                    CurrentLifeStats.SniperShotsFired++;
                    break;
                case UberStrikeItemClass.WeaponSplattergun:
                    TotalStats.SplattergunShotsFired++;
                    CurrentLifeStats.SplattergunShotsFired++;
                    break;
                case UberStrikeItemClass.WeaponMelee:
                    TotalStats.MeleeShotsFired++;
                    CurrentLifeStats.MeleeShotsFired++;
                    break;
                case UberStrikeItemClass.WeaponMachinegun:
                    TotalStats.MachineGunShotsFired++;
                    CurrentLifeStats.MachineGunShotsFired++;
                    break;
                case UberStrikeItemClass.WeaponLauncher:
                    TotalStats.LauncherShotsFired++;
                    CurrentLifeStats.LauncherShotsFired++;
                    break;
                case UberStrikeItemClass.WeaponCannon:
                    TotalStats.CannonShotsFired++;
                    CurrentLifeStats.CannonShotsFired++;
                    break;
            }
        }

        public void IncrementDamageDone(UberStrikeItemClass itemClass, int weaponId, int dmg)
        {
            if (WeaponStats.ContainsKey(weaponId))
                WeaponStats[weaponId].DamageDone += dmg;
            else
            {
                WeaponStats.Add(weaponId, new WeaponStats());
                WeaponStats[weaponId].DamageDone += dmg;
                WeaponStats[weaponId].ItemClass = itemClass;
            }
            switch (itemClass)
            {
                case UberStrikeItemClass.WeaponShotgun:
                    TotalStats.ShotgunDamageDone += dmg;
                    CurrentLifeStats.ShotgunDamageDone += dmg;
                    break;
                case UberStrikeItemClass.WeaponSniperRifle:
                    TotalStats.SniperDamageDone += dmg;
                    CurrentLifeStats.SniperDamageDone += dmg;
                    break;
                case UberStrikeItemClass.WeaponSplattergun:
                    TotalStats.SplattergunDamageDone += dmg;
                    CurrentLifeStats.SplattergunDamageDone += dmg;
                    break;
                case UberStrikeItemClass.WeaponMelee:
                    TotalStats.MeleeDamageDone += dmg;
                    CurrentLifeStats.MeleeDamageDone += dmg;
                    break;
                case UberStrikeItemClass.WeaponMachinegun:
                    TotalStats.MachineGunDamageDone += dmg;
                    CurrentLifeStats.MachineGunDamageDone += dmg;
                    break;
                case UberStrikeItemClass.WeaponLauncher:
                    TotalStats.LauncherDamageDone += dmg;
                    CurrentLifeStats.LauncherDamageDone += dmg;
                    break;
                case UberStrikeItemClass.WeaponCannon:
                    TotalStats.CannonDamageDone += dmg;
                    CurrentLifeStats.CannonDamageDone += dmg;
                    break;
            }
        }

        public void IncrementShotsHit(UberStrikeItemClass itemClass, int weaponId)
        {
            if (WeaponStats.ContainsKey(weaponId))
                WeaponStats[weaponId].ShotsHit++;
            else
            {
                WeaponStats.Add(weaponId, new WeaponStats());
                WeaponStats[weaponId].ShotsHit++;
                WeaponStats[weaponId].ItemClass = itemClass;
            }

            switch (itemClass)
            {
                case UberStrikeItemClass.WeaponShotgun:
                    TotalStats.ShotgunShotsHit++;
                    CurrentLifeStats.ShotgunShotsHit++;
                    break;
                case UberStrikeItemClass.WeaponSniperRifle:
                    TotalStats.SniperShotsHit++;
                    CurrentLifeStats.SniperShotsHit++;
                    break;
                case UberStrikeItemClass.WeaponSplattergun:
                    TotalStats.SplattergunShotsHit++;
                    CurrentLifeStats.SplattergunShotsHit++;
                    break;
                case UberStrikeItemClass.WeaponMelee:
                    TotalStats.MeleeShotsHit++;
                    CurrentLifeStats.MeleeShotsHit++;
                    break;
                case UberStrikeItemClass.WeaponMachinegun:
                    TotalStats.MachineGunShotsHit++;
                    CurrentLifeStats.MachineGunShotsHit++;
                    break;
                case UberStrikeItemClass.WeaponLauncher:
                    TotalStats.LauncherShotsHit++;
                    CurrentLifeStats.LauncherShotsHit++;
                    break;
                case UberStrikeItemClass.WeaponCannon:
                    TotalStats.CannonShotsHit++;
                    CurrentLifeStats.CannonShotsHit++;
                    break;
            }
        }

        public void IncrementPowerUp(PickupItemType itemType, int amount)
        {
            switch (itemType)
            {
                case PickupItemType.Armor:
                    TotalStats.ArmorPickedUp += amount;
                    CurrentLifeStats.ArmorPickedUp += amount;
                    break;
                case PickupItemType.Health:
                    TotalStats.HealthPickedUp += amount;
                    CurrentLifeStats.HealthPickedUp += amount;
                    break;
            }
        }

        public void IncrementKills(UberStrikeItemClass itemClass)
        {

            switch (itemClass)
            {
                case UberStrikeItemClass.WeaponShotgun:
                    TotalStats.ShotgunSplats++;
                    CurrentLifeStats.ShotgunSplats++;
                    break;
                case UberStrikeItemClass.WeaponSniperRifle:
                    TotalStats.SniperKills++;
                    CurrentLifeStats.SniperKills++;
                    break;
                case UberStrikeItemClass.WeaponSplattergun:
                    TotalStats.SplattergunKills++;
                    CurrentLifeStats.SplattergunKills++;
                    break;
                case UberStrikeItemClass.WeaponMelee:
                    TotalStats.MeleeKills++;
                    CurrentLifeStats.MeleeKills++;
                    break;
                case UberStrikeItemClass.WeaponMachinegun:
                    TotalStats.MachineGunKills++;
                    CurrentLifeStats.MachineGunKills++;
                    break;
                case UberStrikeItemClass.WeaponLauncher:
                    TotalStats.LauncherKills++;
                    CurrentLifeStats.LauncherKills++;
                    break;
                case UberStrikeItemClass.WeaponCannon:
                    TotalStats.CannonKills++;
                    CurrentLifeStats.CannonKills++;
                    break;
            }
        }

        public EndOfMatchDataView GetStats(bool hasWon, string matchGuid, List<StatsSummaryView> MVPs)
        {
            EndOfMatchDataView ret = new EndOfMatchDataView();
            ret.HasWonMatch = hasWon;
            ret.MatchGuid = matchGuid;
            ret.MostEffecientWeaponId = GetMostEfficientWeaponId();
            ret.PlayerStatsTotal = TotalStats;
            ret.PlayerStatsBestPerLife = GetBestPerLifeStats();
            ret.PlayerXpEarned = CalculateXp();
            ret.MostValuablePlayers = MVPs;
            return ret;
        }

        public Dictionary<byte, ushort> CalculateXp()
        {
            // TODO: Calculate XP earned.
            Dictionary<byte, ushort> ret = new Dictionary<byte, ushort>
            {
                { 1, 10 },
                { 2, 20 },
                { 3, 30 },
                { 4, 40 },
                { 5, 50 },
                { 6, 60 }
            };
            return ret;
        }

        public StatsCollectionView GetBestPerLifeStats()
        {
            StatsCollectionView ret = new StatsCollectionView();
            StatsPerLife.Add(CurrentLifeStats);
            ret.ArmorPickedUp = StatsPerLife.Max(x => x.ArmorPickedUp);
            ret.HealthPickedUp = StatsPerLife.Max(x => x.HealthPickedUp);
            ret.CannonKills = StatsPerLife.Max(x => x.CannonKills);
            ret.CannonDamageDone = StatsPerLife.Max(x => x.CannonDamageDone);
            ret.CannonShotsFired = StatsPerLife.Max(x => x.CannonShotsFired);
            ret.CannonShotsHit = StatsPerLife.Max(x => x.CannonShotsHit);
            ret.LauncherKills = StatsPerLife.Max(x => x.LauncherKills);
            ret.LauncherDamageDone = StatsPerLife.Max(x => x.LauncherDamageDone);
            ret.LauncherShotsFired = StatsPerLife.Max(x => x.LauncherShotsFired);
            ret.LauncherShotsHit = StatsPerLife.Max(x => x.LauncherShotsHit);
            ret.SniperKills = StatsPerLife.Max(x => x.SniperKills);
            ret.SniperDamageDone = StatsPerLife.Max(x => x.SniperDamageDone);
            ret.SniperShotsFired = StatsPerLife.Max(x => x.SniperShotsFired);
            ret.SniperShotsHit = StatsPerLife.Max(x => x.SniperShotsHit);
            ret.MachineGunKills = StatsPerLife.Max(x => x.MachineGunKills);
            ret.MachineGunDamageDone = StatsPerLife.Max(x => x.MachineGunDamageDone);
            ret.MachineGunShotsFired = StatsPerLife.Max(x => x.MachineGunShotsFired);
            ret.MachineGunShotsHit = StatsPerLife.Max(x => x.MachineGunShotsHit);
            ret.SplattergunKills = StatsPerLife.Max(x => x.SplattergunKills);
            ret.SplattergunDamageDone = StatsPerLife.Max(x => x.SplattergunDamageDone);
            ret.SplattergunShotsFired = StatsPerLife.Max(x => x.SplattergunShotsFired);
            ret.SplattergunShotsHit = StatsPerLife.Max(x => x.SplattergunShotsHit);
            ret.ShotgunSplats = StatsPerLife.Max(x => x.ShotgunSplats);
            ret.ShotgunDamageDone = StatsPerLife.Max(x => x.ShotgunDamageDone);
            ret.ShotgunShotsFired = StatsPerLife.Max(x => x.ShotgunShotsFired);
            ret.ShotgunShotsHit = StatsPerLife.Max(x => x.ShotgunShotsHit);
            ret.MeleeKills = StatsPerLife.Max(x => x.MeleeKills);
            ret.MeleeDamageDone = StatsPerLife.Max(x => x.MeleeDamageDone);
            ret.MeleeShotsFired = StatsPerLife.Max(x => x.MeleeShotsFired);
            ret.MeleeShotsHit = StatsPerLife.Max(x => x.MeleeShotsHit);
            ret.Headshots = StatsPerLife.Max(x => x.Headshots);
            ret.Nutshots = StatsPerLife.Max(x => x.Nutshots);
            ret.Points = StatsPerLife.Max(x => x.Points);
            // idk how they somehow gonna die twice or something
            // it could be .Max but makes more sense to .Min so eh
            ret.Deaths = StatsPerLife.Min(x => x.Deaths);
            ret.Suicides = StatsPerLife.Min(x => x.Suicides);
            return ret;
        }

        public int GetMostEfficientWeaponId()
        {
            // Get the weapon with highest efficiency score.
            // Efficiency score: Accuracy * Damage
            if (WeaponStats.Count > 0)
                return WeaponStats.MaxBy(x => (x.Value.ShotsHit * x.Value.ShotsFired) * x.Value.DamageDone).Key;
            else
                // Splatbat weapon ID. Placeholder.
                return 1;
        }

        public TimeSpan lastKillTime;
        public int killCounter;

        public StatsCollectionView TotalStats { get; set; }
        public StatsCollectionView CurrentLifeStats { get; set; }
        public List<StatsCollectionView> StatsPerLife { get; set; }
        // weapon id, stats
        public Dictionary<int, WeaponStats> WeaponStats { get; set; }

        public string AuthToken { get; set; }
        public ushort Ping { get; set; }
        public GameActor Actor { get; set; }

        /* TODO: Not really sure if we need this. But might want to turn it into a HashSet. */
        public List<int> KnownActors { get; set; }
        public BaseGameRoom Room { get; set; }
        public LoadoutView Loadout { get; set; }
        public UberstrikeUserView Member { get; set; }

        public GamePeerEvents Events => _events;
        public StateMachine<PeerState.Id> State => _state;
    }
}
