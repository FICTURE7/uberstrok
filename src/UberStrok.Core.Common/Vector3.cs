namespace UberStrok.Core.Common
{
    public struct Vector3
    {
        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public float x;
        public float y;
        public float z;

        public override string ToString()
        {
            return $"({x},{y},{z})";
        }
    }
}
