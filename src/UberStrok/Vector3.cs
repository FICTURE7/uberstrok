using System;

namespace UberStrok
{
    public struct Vector3
    {
        public Vector3(float x, float y) : this(x, y, 0)
        {
            /* Space */
        }

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public float X;
        public float Y;
        public float Z;

        public float Magnitude => (float)Math.Sqrt(X * X + Y * Y + Z * Z);

        public static bool operator ==(Vector3 a, Vector3 b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(Vector3 a, Vector3 b)
        {
            return !a.Equals(b);
        }

        public bool Equals(Vector3 a)
        {
            return a.X == X && a.Y == Y && a.Z == Z;
        }

        public override int GetHashCode()
        {
            return X.GetHashCode() ^ Y.GetHashCode() << 2 ^ Z.GetHashCode() >> 2;
        }

        public override string ToString()
        {
            return $"({X},{Y},{Z})";
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is Vector3)
                return Equals((Vector3)obj);
            return false;
        }
    }
}
