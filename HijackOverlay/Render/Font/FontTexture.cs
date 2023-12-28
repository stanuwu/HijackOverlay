using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Windows.Media;
using HijackOverlay.Util;
using OpenGL;
using Brush = System.Drawing.Brush;
using Color = System.Drawing.Color;
using PixelFormat = OpenGL.PixelFormat;

namespace HijackOverlay.Render.Font
{
    public class FontTexture
    {
        private const int Padding = 2;
        private const int CharCount = 128;

        public FontTexture(System.Drawing.Font font, bool antiAlias)
        {
            Id = Gl.GenTexture();
            var typeface = new Typeface(font.FontFamily.Name);
            if (!typeface.TryGetGlyphTypeface(out var glyphTypeface)) throw new FontLoadException(font.Name, font.Size);
            var height = glyphTypeface.Height * font.Size;
            double width = 0;
            double maxWidth = 0;
            var validChars = new List<char>();
            var i = 0;
            var graphics = Graphics.FromImage(new Bitmap(1, 1));
            for (; i < CharCount; i++)
            {
                var character = (char)i;
                if (!glyphTypeface.CharacterToGlyphMap.ContainsKey(character)) continue;
                var glyph = glyphTypeface.CharacterToGlyphMap[character];
                var charWidth = graphics.MeasureString($"{character}", font).Width + glyphTypeface.AdvanceWidths[glyph];
                width += charWidth;
                if (charWidth > maxWidth) maxWidth = charWidth;
                validChars.Add(character);
            }

            Chars = new CharTextureData[i];
            width += Padding * validChars.Count;
            width += Math.Sqrt(width * height) / height * (maxWidth / 2);
            var size = MathUtil.CeilBinaryPower((int)Math.Ceiling(Math.Sqrt(width * (height + Padding))));
            var atlas = new Bitmap(size, size);
            graphics = Graphics.FromImage(atlas);
            Brush blank = new SolidBrush(Color.Transparent);
            Brush white = new SolidBrush(Color.White);
            graphics.FillRectangle(blank, 0, 0, size, size);
            graphics.TextRenderingHint = antiAlias ? TextRenderingHint.AntiAliasGridFit : TextRenderingHint.SystemDefault;
            graphics.SmoothingMode = antiAlias ? SmoothingMode.HighQuality : SmoothingMode.Default;
            graphics.InterpolationMode = antiAlias ? InterpolationMode.HighQualityBilinear : InterpolationMode.Default;
            graphics.CompositingQuality = CompositingQuality.HighQuality;
            graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            float posX = 0;
            var posY = (float)height;
            foreach (var character in validChars)
            {
                var offset = (int)font.Size / 6;
                double charWidth = graphics.MeasureString($"{character}", font).Width - offset;
                if (character == ' ') charWidth += offset * 2;
                if (posX + charWidth > size)
                {
                    posX = 0;
                    posY += (float)height + Padding;
                }

                graphics.DrawString($"{character}", font, white, posX, posY);
                Chars[character] = new CharTextureData((posX + offset) / size, posY / size, (float)(posX + charWidth) / size,
                    (float)(posY + height) / size,
                    (int)Math.Round(charWidth - offset), (int)Math.Round(height));
                posX += (float)(charWidth + Padding + glyphTypeface.AdvanceWidths[glyphTypeface.CharacterToGlyphMap[character]]);
            }

            blank.Dispose();
            white.Dispose();
            graphics.Dispose();

            Gl.BindTexture(TextureTarget.Texture2d, Id);
            Gl.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, Gl.LINEAR);
            Gl.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, Gl.LINEAR);

            Gl.TexImage2D(TextureTarget.Texture2d, Gl.ZERO, InternalFormat.Rgba, size, size, Gl.ZERO, PixelFormat.Rgba, PixelType.UnsignedByte,
                TextureUtil.MakeBuffer(atlas));
        }

        public uint Id { get; }

        public CharTextureData[] Chars { get; }

        public CharTextureData GetCharData(char character)
        {
            if (Chars.Length < character) return CharTextureData.Default;
            var charTextureData = Chars[character];
            return charTextureData;
        }

        public readonly record struct CharTextureData(float U, float V, float U2, float V2, int Width, int Height)
        {
            public static CharTextureData Default { get; } = new(0, 0, 0, 0, 0, 0);
        }
    }
}