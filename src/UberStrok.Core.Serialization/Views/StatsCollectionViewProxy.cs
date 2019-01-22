using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using UberStrok.Core.Views;
using UberStrok.Core.Common;

namespace UberStrok.Core.Serialization.Views
{
    public static class StatsCollectionViewProxy
    {
        public static void Serialize(Stream stream, StatsCollectionView instance)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                Int32Proxy.Serialize(memoryStream, instance.ArmorPickedUp);
                Int32Proxy.Serialize(memoryStream, instance.CannonDamageDone);
                Int32Proxy.Serialize(memoryStream, instance.CannonKills);
                Int32Proxy.Serialize(memoryStream, instance.CannonShotsFired);
                Int32Proxy.Serialize(memoryStream, instance.CannonShotsHit);
                Int32Proxy.Serialize(memoryStream, instance.ConsecutiveSnipes);
                Int32Proxy.Serialize(memoryStream, instance.DamageReceived);
                Int32Proxy.Serialize(memoryStream, instance.Deaths);
                Int32Proxy.Serialize(memoryStream, instance.Headshots);
                Int32Proxy.Serialize(memoryStream, instance.HealthPickedUp);
                Int32Proxy.Serialize(memoryStream, instance.LauncherDamageDone);
                Int32Proxy.Serialize(memoryStream, instance.LauncherKills);
                Int32Proxy.Serialize(memoryStream, instance.LauncherShotsFired);
                Int32Proxy.Serialize(memoryStream, instance.LauncherShotsHit);
                Int32Proxy.Serialize(memoryStream, instance.MachineGunDamageDone);
                Int32Proxy.Serialize(memoryStream, instance.MachineGunKills);
                Int32Proxy.Serialize(memoryStream, instance.MachineGunShotsFired);
                Int32Proxy.Serialize(memoryStream, instance.MachineGunShotsHit);
                Int32Proxy.Serialize(memoryStream, instance.MeleeDamageDone);
                Int32Proxy.Serialize(memoryStream, instance.MeleeKills);
                Int32Proxy.Serialize(memoryStream, instance.MeleeShotsFired);
                Int32Proxy.Serialize(memoryStream, instance.MeleeShotsHit);
                Int32Proxy.Serialize(memoryStream, instance.Nutshots);
                Int32Proxy.Serialize(memoryStream, instance.Points);
                Int32Proxy.Serialize(memoryStream, instance.ShotgunDamageDone);
                Int32Proxy.Serialize(memoryStream, instance.ShotgunShotsFired);
                Int32Proxy.Serialize(memoryStream, instance.ShotgunShotsHit);
                Int32Proxy.Serialize(memoryStream, instance.ShotgunSplats);
                Int32Proxy.Serialize(memoryStream, instance.SniperDamageDone);
                Int32Proxy.Serialize(memoryStream, instance.SniperKills);
                Int32Proxy.Serialize(memoryStream, instance.SniperShotsFired);
                Int32Proxy.Serialize(memoryStream, instance.SniperShotsHit);
                Int32Proxy.Serialize(memoryStream, instance.SplattergunDamageDone);
                Int32Proxy.Serialize(memoryStream, instance.SplattergunKills);
                Int32Proxy.Serialize(memoryStream, instance.SplattergunShotsFired);
                Int32Proxy.Serialize(memoryStream, instance.SplattergunShotsHit);
                Int32Proxy.Serialize(memoryStream, instance.Suicides);
                Int32Proxy.Serialize(memoryStream, instance.Xp);
                memoryStream.WriteTo(stream);
            }
        }

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
    }
}