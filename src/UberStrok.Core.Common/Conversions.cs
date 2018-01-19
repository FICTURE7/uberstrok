using System;

namespace UberStrok.Core.Common
{
    public static class Conversions
    {
        public static float Deg2Rad(float angle)
        {
            return Math.Abs((angle % 360f + 360f) % 360f / 360f);
        }

        public static byte Angle2Byte(float angle)
        {
            return (byte)(255f * Deg2Rad(angle));
        }
    }
}
