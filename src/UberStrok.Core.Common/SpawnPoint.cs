namespace UberStrok.Core.Common
{
    public struct SpawnPoint
    {
        public Vector3Old Position;
        public byte Rotation;

        public SpawnPoint(Vector3Old position, byte rotation)
        {
            Position = position;
            Rotation = rotation;
        }

        public override string ToString()
        {
            return $"{Position}:{Rotation}";
        }
    }
}
