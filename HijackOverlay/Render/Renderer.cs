using System;
using System.Drawing;
using System.Linq;
using HijackOverlay.Render.Buffer;
using HijackOverlay.Render.Shader;
using HijackOverlay.Render.Texture;
using HijackOverlay.Util;
using OpenGL;

namespace HijackOverlay.Render
{
    public static class Renderer
    {
        public static void SetBlend()
        {
            Gl.Enable(EnableCap.Blend);
            Gl.BlendFuncSeparate(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha, BlendingFactor.One, BlendingFactor.OneMinusSrcAlpha);
        }

        public static BufferBuilder StartPositionColorLines()
        {
            SetBlend();
            return new BufferBuilder(PrimitiveType.Lines, VertexModes.PositionColor, ShaderManager.Instance.PositionColorShader);
        }

        public static BufferBuilder StartPositionColorTris()
        {
            SetBlend();
            return new BufferBuilder(PrimitiveType.Triangles, VertexModes.PositionColor, ShaderManager.Instance.PositionColorShader);
        }

        public static BufferBuilder StartPositionTextureTris(GlTexture texture)
        {
            SetBlend();
            var bufferBuilder = new BufferBuilder(PrimitiveType.Triangles, VertexModes.PositionTexture, ShaderManager.Instance.PositionTextureShader);
            bufferBuilder.SetTexture(texture.Id);
            return bufferBuilder;
        }

        public static BufferBuilder StartPositionColorTextureTris(GlTexture texture)
        {
            SetBlend();
            var bufferBuilder = new BufferBuilder(PrimitiveType.Triangles, VertexModes.PositionColorTexture, ShaderManager.Instance.PositionColorTextureShader);
            bufferBuilder.SetTexture(texture.Id);
            return bufferBuilder;
        }

        public static void End(BufferBuilder bufferBuilder)
        {
            bufferBuilder.Draw();
            Gl.Disable(EnableCap.Blend);
        }

        public static void Scissor(float x, float y, float width, float height)
        {
            Gl.Scissor((int)x, (int)y, (int)width, (int)height);
        }

        public static void DrawRoundedColorRect(float x, float y, float width, float height, float radius, Color color)
        {
            DrawRoundedColorRect(x, y, width, height, radius, color, color, color, color);
        }
        
        public static void DrawRoundedColorRect(float x, float y, float width, float height, float radius, Color color00, Color color01, Color color10, Color color11)
        {
            SetBlend();
            var shader = ShaderManager.Instance.RoundedPositionColorShader;
            BufferBuilder bufferBuilder = new BufferBuilder(PrimitiveType.Triangles, VertexModes.PositionColor, shader);
            shader.Uniform1F("u_radius", radius);
            shader.Uniform2F("u_size", width, height);
            shader.Uniform2F("u_position", x, y);
            BufferColorRect(bufferBuilder, x, y, width, height, color00, color01, color10, color11);
            End(bufferBuilder);
        }
        
        public static void DrawColorCircle(float x, float y, float radius, Color color)
        {
            DrawColorCircle(x, y, radius, color, color, color, color);
        }
        
        public static void DrawColorCircle(float x, float y, float radius, Color color00, Color color01, Color color10, Color color11)
        {
            DrawRoundedColorRect(x, y, radius, radius, radius/2 + 4, color00, color01, color10, color11);
        }
        
        public static void DrawRoundedOutlineColorRect(float x, float y, float width, float height, float radius, float thickness, Color color)
        {
            DrawRoundedOutlineColorRect(x, y, width, height, radius, thickness, color, color, color, color);
        }
        
        public static void DrawRoundedOutlineColorRect(float x, float y, float width, float height, float radius, float thickness, Color color00, Color color01, Color color10, Color color11)
        {
            SetBlend();
            var shader = ShaderManager.Instance.RoundedOutlinePositionColorShader;
            BufferBuilder bufferBuilder = new BufferBuilder(PrimitiveType.Triangles, VertexModes.PositionColor, shader);
            shader.Uniform1F("u_radius", radius);
            shader.Uniform1F("u_thickness", thickness);
            shader.Uniform2F("u_size", width, height);
            shader.Uniform2F("u_position", x, y);
            BufferColorRect(bufferBuilder, x, y, width, height, color00, color01, color10, color11);
            End(bufferBuilder);
        }
        
        public static void DrawOutlineColorCircle(float x, float y, float radius, float thickness, Color color)
        {
            DrawOutlineColorCircle(x, y, radius, thickness, color, color, color, color);
        }
        
        public static void DrawOutlineColorCircle(float x, float y, float radius, float thickness, Color color00, Color color01, Color color10, Color color11)
        {
            DrawRoundedOutlineColorRect(x, y, radius, radius, radius/2 + 4, thickness, color00, color01, color10, color11);
        }

        public static void BufferColorRect(BufferBuilder bufferBuilder, float x, float y, float width, float height, Color color00, Color color01,
            Color color10,
            Color color11)
        {
            var x2 = x + width;
            var y2 = y + height;
            bufferBuilder.Vert(x, y2).Color(color00).End();
            bufferBuilder.Vert(x, y).Color(color01).End();
            bufferBuilder.Vert(x2, y).Color(color11).End();
            bufferBuilder.Vert(x2, y).Color(color11).End();
            bufferBuilder.Vert(x2, y2).Color(color10).End();
            bufferBuilder.Vert(x, y2).Color(color00).End();
        }

        public static void BufferColorRect(BufferBuilder bufferBuilder, float x, float y, float width, float height, Color color)
        {
            var x2 = x + width;
            var y2 = y + height;
            bufferBuilder.Vert(x, y2).Color(color).End();
            bufferBuilder.Vert(x, y).Color(color).End();
            bufferBuilder.Vert(x2, y).Color(color).End();
            bufferBuilder.Vert(x2, y).Color(color).End();
            bufferBuilder.Vert(x2, y2).Color(color).End();
            bufferBuilder.Vert(x, y2).Color(color).End();
        }

        public static void BufferTextureRect(BufferBuilder bufferBuilder, float x, float y, float width, float height)
        {
            var x2 = x + width;
            var y2 = y + height;
            bufferBuilder.Vert(x, y2).UV(0, 0).End();
            bufferBuilder.Vert(x, y).UV(0, 1).End();
            bufferBuilder.Vert(x2, y).UV(1, 1).End();
            bufferBuilder.Vert(x2, y).UV(1, 1).End();
            bufferBuilder.Vert(x2, y2).UV(1, 0).End();
            bufferBuilder.Vert(x, y2).UV(0, 0).End();
        }

        public static void BufferTextureUvRect(BufferBuilder bufferBuilder, float x, float y, float width, float height, float u, float v, float u2, float v2)
        {
            var x2 = x + width;
            var y2 = y + height;
            bufferBuilder.Vert(x, y2).UV(u, v).End();
            bufferBuilder.Vert(x, y).UV(u, v2).End();
            bufferBuilder.Vert(x2, y).UV(u2, v2).End();
            bufferBuilder.Vert(x2, y).UV(u2, v2).End();
            bufferBuilder.Vert(x2, y2).UV(u2, v).End();
            bufferBuilder.Vert(x, y2).UV(u, v).End();
        }

        public static void BufferTextureColorRect(BufferBuilder bufferBuilder, float x, float y, float width, float height, Color color)
        {
            var x2 = x + width;
            var y2 = y + height;
            bufferBuilder.Vert(x, y2).Color(color).UV(0, 0).End();
            bufferBuilder.Vert(x, y).Color(color).UV(0, 1).End();
            bufferBuilder.Vert(x2, y).Color(color).UV(1, 1).End();
            bufferBuilder.Vert(x2, y).Color(color).UV(1, 1).End();
            bufferBuilder.Vert(x2, y2).Color(color).UV(1, 0).End();
            bufferBuilder.Vert(x, y2).Color(color).UV(0, 0).End();
        }

        public static void BufferTextureColorRect(BufferBuilder bufferBuilder, float x, float y, float width, float height, Color color00, Color color01,
            Color color10, Color color11)
        {
            var x2 = x + width;
            var y2 = y + height;
            bufferBuilder.Vert(x, y2).Color(color00).UV(0, 0).End();
            bufferBuilder.Vert(x, y).Color(color01).UV(0, 1).End();
            bufferBuilder.Vert(x2, y).Color(color11).UV(1, 1).End();
            bufferBuilder.Vert(x2, y).Color(color11).UV(1, 1).End();
            bufferBuilder.Vert(x2, y2).Color(color10).UV(1, 0).End();
            bufferBuilder.Vert(x, y2).Color(color00).UV(0, 0).End();
        }

        public static void BufferTextureUvColorRect(BufferBuilder bufferBuilder, float x, float y, float width, float height, float u, float v, float u2,
            float v2, Color color)
        {
            var x2 = x + width;
            var y2 = y + height;
            bufferBuilder.Vert(x, y2).Color(color).UV(u, v).End();
            bufferBuilder.Vert(x, y).Color(color).UV(u, v2).End();
            bufferBuilder.Vert(x2, y).Color(color).UV(u2, v2).End();
            bufferBuilder.Vert(x2, y).Color(color).UV(u2, v2).End();
            bufferBuilder.Vert(x2, y2).Color(color).UV(u2, v).End();
            bufferBuilder.Vert(x, y2).Color(color).UV(u, v).End();
        }

        public static void BufferTextureUvColorRect(BufferBuilder bufferBuilder, float x, float y, float width, float height, float u, float v, float u2,
            float v2, Color color00, Color color01, Color color10, Color color11)
        {
            var x2 = x + width;
            var y2 = y + height;
            bufferBuilder.Vert(x, y2).Color(color00).UV(u, v).End();
            bufferBuilder.Vert(x, y).Color(color01).UV(u, v2).End();
            bufferBuilder.Vert(x2, y).Color(color11).UV(u2, v2).End();
            bufferBuilder.Vert(x2, y).Color(color11).UV(u2, v2).End();
            bufferBuilder.Vert(x2, y2).Color(color10).UV(u2, v).End();
            bufferBuilder.Vert(x, y2).Color(color00).UV(u, v).End();
        }

        public static void BufferColorLine(BufferBuilder bufferBuilder, float x, float y, float x2, float y2, Color color)
        {
            bufferBuilder.Vert(x, y).Color(color).End();
            bufferBuilder.Vert(x2, y2).Color(color).End();
        }

        public static void BufferColorLine(BufferBuilder bufferBuilder, float x, float y, float x2, float y2, Color color, Color color2)
        {
            bufferBuilder.Vert(x, y).Color(color).End();
            bufferBuilder.Vert(x2, y2).Color(color2).End();
        }

        public static void BufferColorLine(BufferBuilder bufferBuilder, float x, float y, float x2, float y2, params Color[] colors)
        {
            switch (colors.Length)
            {
                case 0:
                    BufferColorLine(bufferBuilder, x, y, x2, y2, Color.Black);
                    return;
                case 1:
                    BufferColorLine(bufferBuilder, x, y, x2, y2, colors[0]);
                    return;
                case 2:
                    BufferColorLine(bufferBuilder, x, y, x2, y2, colors[0], colors[1]);
                    return;
            }

            var dirX = x2 - x;
            var dirY = y2 - y;
            dirX /= colors.Length - 1;
            dirY /= colors.Length - 1;
            for (var i = 0; i < colors.Length - 1; i++)
            {
                bufferBuilder.Vert(x + dirX * i, y + dirY * i).Color(colors[i]).End();
                bufferBuilder.Vert(x + dirX * (i + 1), y + dirY * (i + 1)).Color(colors[i + 1]).End();
            }
        }

        public static void BufferColorGradientLineGroup(BufferBuilder bufferBuilder, float[] xs, float[] ys, float[] x2s, float[] y2s, params Color[] colors)
        {
            if (xs.Length != ys.Length || ys.Length != x2s.Length || x2s.Length != y2s.Length) return;
            var minY = Math.Min(ys.Min(), y2s.Min());
            var maxY = Math.Max(ys.Max(), y2s.Max());
            var distance = maxY - minY;

            for (var i = 0; i < xs.Length; i++)
            {
                var x = xs[i];
                var y = ys[i];
                var x2 = x2s[i];
                var y2 = y2s[i];
                var parts = colors.Length;
                var partMin = Math.Min(y, y2);
                var partMax = Math.Max(y, y2);
                var partDist = partMax - partMin;
                partDist /= parts - 1;
                var partColors = new Color[parts];
                for (var j = 0; j < parts; j++)
                {
                    var pos = maxY - partMin - partDist * j;
                    partColors[parts - j - 1] = ColorUtil.MultiGradientPoint(pos / distance, colors);
                }

                if (partMin == y) BufferColorLine(bufferBuilder, x2, y2, x, y, partColors);
                else BufferColorLine(bufferBuilder, x, y, x2, y2, partColors);
            }
        }

        public static void BufferOutlineColorRect(BufferBuilder bufferBuilder, float x, float y, float width, float height, Color color)
        {
            var x2 = x + width;
            var y2 = y + height;
            BufferColorLine(bufferBuilder, x, y, x2, y, color);
            BufferColorLine(bufferBuilder, x, y, x, y2, color);
            BufferColorLine(bufferBuilder, x2, y, x2, y2, color);
            BufferColorLine(bufferBuilder, x, y2, x2, y2, color);
        }

        public static void BufferOutlineColorRect(BufferBuilder bufferBuilder, float x, float y, float width, float height, Color color00, Color color01,
            Color color10, Color color11)
        {
            var x2 = x + width;
            var y2 = y + height;
            BufferColorLine(bufferBuilder, x, y, x2, y, color01, color11);
            BufferColorLine(bufferBuilder, x, y, x, y2, color01, color00);
            BufferColorLine(bufferBuilder, x2, y, x2, y2, color11, color10);
            BufferColorLine(bufferBuilder, x, y2, x2, y2, color00, color10);
        }
    }
}