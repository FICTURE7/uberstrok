using System;

namespace UberStrok.Core.Common
{
    public struct Vector3
    {
        public static readonly Vector3 Zero = new Vector3(0, 0, 0);

        public Vector3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public float x;
        public float y;
        public float z;

        public float Magnitude => (float)Math.Sqrt(x * x + y * y + z * z);

        public Vector3 Normalized
        {
            get
            {
                float magnitude = Magnitude;
                if (magnitude > 1E-05f)
                    return this / magnitude;
                return new Vector3(0, 0, 0);
            }
        }

        public static float Angle(Vector3 from, Vector3 to)
        {
            return (float)Math.Acos(MathUtils.Clamp(Dot(from.Normalized, to.Normalized), -1f, 1f)) * 57.29578f;
        }

        public static float Dot(Vector3 a, Vector3 b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }

        public static Vector3 operator /(Vector3 a, float d)
        {
            return new Vector3(a.x / d, a.y / d, a.z / d);
        }

        public static Vector3 operator *(Vector3 a, int d)
        {
            return new Vector3(a.x * d, a.y * d, a.z * d);
        }

        public static Vector3 operator -(Vector3 a)
        {
            return new Vector3(-a.x, -a.y, -a.z);
        }

        public static Vector3 operator -(Vector3 a, Vector3 b)
        {
            return new Vector3(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public override string ToString()
        {
            return $"({x},{y},{z})";
        }
    }
}
