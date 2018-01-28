using System;
using UberStrok.Core.Common;

namespace UberStrok.Realtime.Server.Game
{
    public class PlayerRespawnedEventArgs : EventArgs
    {
        public GamePeer Player { get; set; }
    }

    public class PlayerJoinedEventArgs : EventArgs
    {
        public GamePeer Player { get; set; }
        public TeamID Team { get; set; }
    }

    public class PlayerLeftEventArgs : EventArgs
    {
        public GamePeer Player { get; set; }
    }

    public class PlayerKilledEventArgs : EventArgs
    {
        public int VictimCmid { get; set; }
        public int AttackerCmid { get; set; }
        public UberStrikeItemClass ItemClass { get; set; }
        public ushort Damage { get; set; }
        public BodyPart Part { get; set; }
        public Vector3 Direction { get; set; }
    }
}
