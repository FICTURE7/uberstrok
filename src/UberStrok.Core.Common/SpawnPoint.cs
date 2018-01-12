namespace UberStrok.Core.Common
{
    public struct SpawnPoint
    {
        public Vector3 Position;
        public byte Rotation;

        public SpawnPoint(Vector3 position, byte rotation)
        {
            Position = position;
            Rotation = rotation;
        }
    }
}
