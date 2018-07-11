using System;

namespace UberStrok.Core.Common
{
    public struct Vector3Old
    {
        public Vector3Old(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public float x;
        public float y;
        public float z;

        public float Magnitude => (float)Math.Sqrt(x * x + y * y + z * z);

        public Vector3Old Normalized
        {
            get
            {
                float magnitude = Magnitude;
                if (magnitude > 1E-05f)
                    return this / magnitude;
                return new Vector3Old(0, 0, 0);
            }
        }

        public static float Angle(Vector3Old from, Vector3Old to)
        {
            return (float)Math.Acos(MathUtils.Clamp(Dot(from.Normalized, to.Normalized), -1f, 1f)) * 57.29578f;
        }

        public static float Dot(Vector3Old a, Vector3Old b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }

        public static Vector3Old operator /(Vector3Old a, float d)
        {
            return new Vector3Old(a.x / d, a.y / d, a.z / d);
        }

        public static Vector3Old operator *(Vector3Old a, int d)
        {
            return new Vector3Old(a.x * d, a.y * d, a.z * d);
        }

        public static Vector3Old operator -(Vector3Old a)
        {
            return new Vector3Old(-a.x, -a.y, -a.z);
        }

        public static Vector3Old operator -(Vector3Old a, Vector3Old b)
        {
            return new Vector3Old(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public override string ToString()
        {
            return $"({x},{y},{z})";
        }
    }
}
