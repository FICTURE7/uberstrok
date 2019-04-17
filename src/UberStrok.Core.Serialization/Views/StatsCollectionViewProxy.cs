using System.IO;
using UberStrok.Core.Views;

namespace UberStrok.Core.Serialization.Views
{
    public static class StatsCollectionViewProxy
    {
        public static StatsCollectionView Deserialize(Stream bytes)
        {
            return new StatsCollectionView
            {
                ArmorPickedUp = Int32Proxy.Deserialize(bytes),
                CannonDamageDone = Int32Proxy.Deserialize(bytes),
                CannonKills = Int32Proxy.Deserialize(bytes),
                CannonShotsFired = Int32Proxy.Deserialize(bytes),
                CannonShotsHit = Int32Proxy.Deserialize(bytes),
                ConsecutiveSnipes = Int32Proxy.Deserialize(bytes),
                DamageReceived = Int32Proxy.Deserialize(bytes),
                Deaths = Int32Proxy.Deserialize(bytes),
                Headshots = Int32Proxy.Deserialize(bytes),
                HealthPickedUp = Int32Proxy.Deserialize(bytes),
                LauncherDamageDone = Int32Proxy.Deserialize(bytes),
                LauncherKills = Int32Proxy.Deserialize(bytes),
                LauncherShotsFired = Int32Proxy.Deserialize(bytes),
                LauncherShotsHit = Int32Proxy.Deserialize(bytes),
                MachineGunDamageDone = Int32Proxy.Deserialize(bytes),
                MachineGunKills = Int32Proxy.Deserialize(bytes),
                MachineGunShotsFired = Int32Proxy.Deserialize(bytes),
                MachineGunShotsHit = Int32Proxy.Deserialize(bytes),
                MeleeDamageDone = Int32Proxy.Deserialize(bytes),
                MeleeKills = Int32Proxy.Deserialize(bytes),
                MeleeShotsFired = Int32Proxy.Deserialize(bytes),
                MeleeShotsHit = Int32Proxy.Deserialize(bytes),
                Nutshots = Int32Proxy.Deserialize(bytes),
                Points = Int32Proxy.Deserialize(bytes),
                ShotgunDamageDone = Int32Proxy.Deserialize(bytes),
                ShotgunShotsFired = Int32Proxy.Deserialize(bytes),
                ShotgunShotsHit = Int32Proxy.Deserialize(bytes),
                ShotgunSplats = Int32Proxy.Deserialize(bytes),
                SniperDamageDone = Int32Proxy.Deserialize(bytes),
                SniperKills = Int32Proxy.Deserialize(bytes),
                SniperShotsFired = Int32Proxy.Deserialize(bytes),
                SniperShotsHit = Int32Proxy.Deserialize(bytes),
                SplattergunDamageDone = Int32Proxy.Deserialize(bytes),
                SplattergunKills = Int32Proxy.Deserialize(bytes),
                SplattergunShotsFired = Int32Proxy.Deserialize(bytes),
                SplattergunShotsHit = Int32Proxy.Deserialize(bytes),
                Suicides = Int32Proxy.Deserialize(bytes),
                Xp = Int32Proxy.Deserialize(bytes)
            };
        }

        public static void Serialize(Stream stream, StatsCollectionView instance)
        {
            using (var bytes = new MemoryStream())
            {
                Int32Proxy.Serialize(bytes, instance.ArmorPickedUp);
                Int32Proxy.Serialize(bytes, instance.CannonDamageDone);
                Int32Proxy.Serialize(bytes, instance.CannonKills);
                Int32Proxy.Serialize(bytes, instance.CannonShotsFired);
                Int32Proxy.Serialize(bytes, instance.CannonShotsHit);
                Int32Proxy.Serialize(bytes, instance.ConsecutiveSnipes);
                Int32Proxy.Serialize(bytes, instance.DamageReceived);
                Int32Proxy.Serialize(bytes, instance.Deaths);
                Int32Proxy.Serialize(bytes, instance.Headshots);
                Int32Proxy.Serialize(bytes, instance.HealthPickedUp);
                Int32Proxy.Serialize(bytes, instance.LauncherDamageDone);
                Int32Proxy.Serialize(bytes, instance.LauncherKills);
                Int32Proxy.Serialize(bytes, instance.LauncherShotsFired);
                Int32Proxy.Serialize(bytes, instance.LauncherShotsHit);
                Int32Proxy.Serialize(bytes, instance.MachineGunDamageDone);
                Int32Proxy.Serialize(bytes, instance.MachineGunKills);
                Int32Proxy.Serialize(bytes, instance.MachineGunShotsFired);
                Int32Proxy.Serialize(bytes, instance.MachineGunShotsHit);
                Int32Proxy.Serialize(bytes, instance.MeleeDamageDone);
                Int32Proxy.Serialize(bytes, instance.MeleeKills);
                Int32Proxy.Serialize(bytes, instance.MeleeShotsFired);
                Int32Proxy.Serialize(bytes, instance.MeleeShotsHit);
                Int32Proxy.Serialize(bytes, instance.Nutshots);
                Int32Proxy.Serialize(bytes, instance.Points);
                Int32Proxy.Serialize(bytes, instance.ShotgunDamageDone);
                Int32Proxy.Serialize(bytes, instance.ShotgunShotsFired);
                Int32Proxy.Serialize(bytes, instance.ShotgunShotsHit);
                Int32Proxy.Serialize(bytes, instance.ShotgunSplats);
                Int32Proxy.Serialize(bytes, instance.SniperDamageDone);
                Int32Proxy.Serialize(bytes, instance.SniperKills);
                Int32Proxy.Serialize(bytes, instance.SniperShotsFired);
                Int32Proxy.Serialize(bytes, instance.SniperShotsHit);
                Int32Proxy.Serialize(bytes, instance.SplattergunDamageDone);
                Int32Proxy.Serialize(bytes, instance.SplattergunKills);
                Int32Proxy.Serialize(bytes, instance.SplattergunShotsFired);
                Int32Proxy.Serialize(bytes, instance.SplattergunShotsHit);
                Int32Proxy.Serialize(bytes, instance.Suicides);
                Int32Proxy.Serialize(bytes, instance.Xp);
                bytes.WriteTo(stream);
            }
        }
    }
}