namespace UberStrok.Core.Common
{
    public struct Color
    {
        public Color(float r, float g, float b)
        {
            R = r;
            G = g;
            B = b;
        }

        public float R;
        public float G;
        public float B;

        public override bool Equals(object obj)
        {
            return obj is Color && this == (Color)obj;
        }

        public static bool operator ==(Color a, Color b)
        {
            return a.R == b.R && a.G == b.G && a.B == b.B;
        }

        public static bool operator !=(Color a, Color b)
        {
            return !(a == b);
        }
    }
}
