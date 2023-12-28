using System.Drawing;
using System.Linq;
using HijackOverlay.Render.Buffer;
using HijackOverlay.Render.Shader;
using HijackOverlay.Util;
using OpenGL;

namespace HijackOverlay.Render.Font
{
    public class FontRenderer
    {
        public FontRenderer(System.Drawing.Font font, bool antiAlias = true)
        {
            Font = new GlFont(font, antiAlias);
        }

        public FontRenderer(string name, int size, FontStyle fontStyle = FontStyle.Regular, bool antiAlias = true)
        {
            var font = new System.Drawing.Font(name, size, fontStyle, GraphicsUnit.Pixel);
            Font = new GlFont(font, antiAlias);
        }

        private GlFont Font { get; }

        public void DrawStringCentered(string text, float x, float y, Color color00, Color color01, Color color10, Color color11)
        {
            DrawString(text, x - GetWidth(text) / 2f, y - GetHeight() / 2f, color00, color01, color10, color11);
        }

        public void DrawStringCentered(string text, float x, float y, params Color[] colors)
        {
            DrawString(text, x - GetWidth(text) / 2f, y - GetHeight() / 2f, colors);
        }

        public void DrawString(string text, float x, float y, Color color00, Color color01, Color color10, Color color11)
        {
            DrawString(text, x, y, new[] { color00, color10 }, new[] { color01, color11 });
        }

        public void DrawString(string text, float x, float y, params Color[] colors)
        {
            DrawString(text, x, y, colors, colors);
        }

        private void DrawString(string text, float x, float y, Color[] top, Color[] bottom)
        {
            float width = GetWidth(text);

            Gl.Enable(EnableCap.Blend);
            Gl.BlendFuncSeparate(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha, BlendingFactor.One, BlendingFactor.OneMinusSrcAlpha);
            var bufferBuilder = new BufferBuilder(PrimitiveType.Triangles, VertexModes.PositionColorTexture,
                ShaderManager.Instance.PositionColorTextureShader);
            bufferBuilder.SetTexture(Font.Texture.Id);
            var xOffset = x;
            foreach (var character in text)
            {
                var charTextureData = Font.Texture.GetCharData(character);
                var gradientPos = (xOffset - x) / width;
                var gradientPos2 = (xOffset - x + charTextureData.Width) / width;
                var gradient00 = ColorUtil.MultiGradientPoint(gradientPos, top);
                var gradient01 = ColorUtil.MultiGradientPoint(gradientPos, bottom);
                var gradient10 = ColorUtil.MultiGradientPoint(gradientPos2, top);
                var gradient11 = ColorUtil.MultiGradientPoint(gradientPos2, bottom);
                xOffset = DrawChar(bufferBuilder, charTextureData, xOffset, y, gradient00, gradient01, gradient10, gradient11);
            }

            bufferBuilder.Draw();
            Gl.Disable(EnableCap.Blend);
        }

        private float DrawChar(BufferBuilder bufferBuilder, FontTexture.CharTextureData charTextureData, float x, float y, Color color00, Color color01,
            Color color10, Color color11)
        {
            var x2 = x + charTextureData.Width;
            var y2 = y + charTextureData.Height;

            bufferBuilder.Vert(x, y2).Color(color00).UV(charTextureData.U, charTextureData.V).End();
            bufferBuilder.Vert(x, y).Color(color01).UV(charTextureData.U, charTextureData.V2).End();
            bufferBuilder.Vert(x2, y).Color(color11).UV(charTextureData.U2, charTextureData.V2).End();
            bufferBuilder.Vert(x2, y).Color(color11).UV(charTextureData.U2, charTextureData.V2).End();
            bufferBuilder.Vert(x2, y2).Color(color10).UV(charTextureData.U2, charTextureData.V).End();
            bufferBuilder.Vert(x, y2).Color(color00).UV(charTextureData.U, charTextureData.V).End();

            return x2;
        }

        public int GetWidth(string text)
        {
            return text.Sum(character => Font.Texture.GetCharData(character).Width);
        }

        public int GetHeight()
        {
            return Font.Texture.GetCharData('0').Height;
        }
    }
}