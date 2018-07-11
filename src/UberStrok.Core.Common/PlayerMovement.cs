using System;

namespace UberStrok.Core.Common
{
    [Serializable]
    public class PlayerMovement
    {
        public byte Number { get; set; }
        public Vector3Old Position { get; set; }
        public Vector3Old Velocity { get; set; }
        public byte HorizontalRotation { get; set; }
        public byte VerticalRotation { get; set; }
        public byte KeyState { get; set; }
        public byte MovementState { get; set; }
    }
}
