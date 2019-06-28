using System.IO;
using UberStrok.Core.Common;

namespace UberStrok.Core.Serialization.Common
{
    public static class PlayerMovementProxy
    {
        public static void Serialize(Stream stream, PlayerMovement instance)
        {
            using (var bytes = new MemoryStream())
            {
                ByteProxy.Serialize(bytes, instance.HorizontalRotation);
                ByteProxy.Serialize(bytes, instance.KeyState);
                ByteProxy.Serialize(bytes, instance.MovementState);
                ByteProxy.Serialize(bytes, instance.PlayerId);
                ShortVector3Proxy.Serialize(bytes, instance.Position);
                ShortVector3Proxy.Serialize(bytes, instance.Velocity);
                ByteProxy.Serialize(bytes, instance.VerticalRotation);
                bytes.WriteTo(stream);
            }
        }

        public static PlayerMovement Deserialize(Stream bytes)
        {
            return new PlayerMovement
            {
                HorizontalRotation = ByteProxy.Deserialize(bytes),
                KeyState = ByteProxy.Deserialize(bytes),
                MovementState = ByteProxy.Deserialize(bytes),
                PlayerId = ByteProxy.Deserialize(bytes),
                Position = ShortVector3Proxy.Deserialize(bytes),
                Velocity = ShortVector3Proxy.Deserialize(bytes),
                VerticalRotation = ByteProxy.Deserialize(bytes)
            };
        }
    }
}
