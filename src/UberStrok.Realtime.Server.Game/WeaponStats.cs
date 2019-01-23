using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UberStrok.Core.Common;

namespace UberStrok.Realtime.Server.Game
{
    public class WeaponStats
    {
        public int DamageDone { get; set; }
        public int ShotsHit { get; set; }
        public int ShotsFired { get; set; }
        public UberStrikeItemClass ItemClass { get; set; }
        public int Kills { get; set; }
    }
}