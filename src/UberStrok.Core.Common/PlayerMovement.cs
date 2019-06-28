using System;

namespace UberStrok.Core.Common
{
    [Serializable]
    public class PlayerMovement
    {
        /* Original identifier `Number`. */
        public byte PlayerId { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Velocity { get; set; }
        public byte HorizontalRotation { get; set; }
        public byte VerticalRotation { get; set; }
        public byte KeyState { get; set; }
        public byte MovementState { get; set; }
    }
}
