namespace HijackOverlay.Util
{
    public static class MathUtil
    {
        public static int CeilBinaryPower(int i)
        {
            i--;
            i |= i >> 1;
            i |= i >> 2;
            i |= i >> 4;
            i |= i >> 8;
            i |= i >> 16;
            i++;
            return i;
        }

        public static float Clamp(float value, float min, float max)
        {
            if (value < min) return min;
            return value > max ? max : value;
        }
    }
}