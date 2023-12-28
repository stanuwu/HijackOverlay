using System.Collections.Generic;
using System.Drawing;

namespace HijackOverlay.Util
{
    public static class TextureUtil
    {
        public static byte[] MakeBuffer(Bitmap image)
        {
            var data = new List<byte>();

            for (var y = 0; y < image.Height; ++y)
            for (var x = 0; x < image.Width; ++x)
            {
                var pixel = image.GetPixel(x, y).ToArgb();
                data.Add((byte)((pixel >> 16) & 0xFF));
                data.Add((byte)((pixel >> 8) & 0xFF));
                data.Add((byte)(pixel & 0xFF));
                data.Add((byte)((pixel >> 24) & 0xFF));
            }

            return data.ToArray();
        }
    }
}