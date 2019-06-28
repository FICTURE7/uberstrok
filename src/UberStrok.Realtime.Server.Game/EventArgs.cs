using System;
using UberStrok.Core.Common;

namespace UberStrok.Realtime.Server.Game
{
    public class PlayerRespawnedEventArgs : EventArgs
    {
        public GameActor Player { get; set; }
    }

    public class PlayerJoinedEventArgs : EventArgs
    {
        public GameActor Player { get; set; }
        public TeamID Team { get; set; }
    }

    public class PlayerLeftEventArgs : EventArgs
    {
        public GameActor Player { get; set; }
    }

    public class PlayerKilledEventArgs : EventArgs
    {
        public GameActor Victim { get; set; }
        public GameActor Attacker { get; set; }
        public UberStrikeItemClass ItemClass { get; set; }
        public ushort Damage { get; set; }
        public BodyPart Part { get; set; }
        public Vector3 Direction { get; set; }
    }
}
