using System;

namespace HijackOverlay.Render.Font
{
    public class FontLoadException : Exception
    {
        public FontLoadException(string font, float size) : base($"Font {font} {size} failed to load")
        {
        }
    }
}