using System;
using System.Drawing;

namespace HijackOverlay.Util
{
    public static class ColorUtil
    {
        public static Color MultiGradientPoint(float position, params Color[] colors)
        {
            switch (colors.Length)
            {
                case 0:
                    return Color.Black;
                case 1:
                    return colors[0];
            }

            position = MathUtil.Clamp(position, 0, 1);
            switch (position)
            {
                case 0:
                    return colors[0];
                case 1:
                    return colors[colors.Length - 1];
            }

            var index = position * (colors.Length - 1);
            var prev = (int)index;
            var next = (int)Math.Ceiling(index);
            var rest = index - prev;
            if (rest == 0) return colors[prev];
            return GradientPoint(colors[prev], colors[next], rest);
        }

        public static Color GradientPoint(Color color1, Color color2, float position)
        {
            position = MathUtil.Clamp(position, 0, 1);
            return Color.FromArgb(
                Interpolate(color1.A, color2.A, position),
                Interpolate(color1.R, color2.R, position),
                Interpolate(color1.G, color2.G, position),
                Interpolate(color1.B, color2.B, position)
            );
        }

        private static int Interpolate(int color1, int color2, float position)
        {
            return (int)(color2 * position + color1 * (1 - position));
        }
    }
}