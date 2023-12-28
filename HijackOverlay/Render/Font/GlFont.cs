namespace HijackOverlay.Render.Font
{
    public class GlFont
    {
        public GlFont(System.Drawing.Font font, bool antiAlias)
        {
            Font = font;
            Texture = new FontTexture(font, antiAlias);
        }

        public System.Drawing.Font Font { get; }
        public FontTexture Texture { get; }
    }
}