using System;
using System.Collections.Generic;
using System.Drawing;
using HijackOverlay.Render.Shader;
using OpenGL;

namespace HijackOverlay.Render.Buffer
{
    public class BufferBuilder
    {
        public BufferBuilder(PrimitiveType drawMode, VertexModes vertexMode, GlShader shader)
        {
            DrawMode = drawMode;
            VertexMode = vertexMode;
            Shader = shader;
            Vertices = new List<float>();
            Colors = new List<float>();
            UVs = new List<float>();
        }

        private PrimitiveType DrawMode { get; }
        private VertexModes VertexMode { get; }
        private GlShader Shader { get; }
        private uint TextureId { get; set; }

        private bool ColorDone { get; set; }
        private float R { get; set; }
        private float G { get; set; }
        private float B { get; set; }
        private float A { get; set; }

        private bool PositionDone { get; set; }
        private float X { get; set; }
        private float Y { get; set; }

        private bool TextureDone { get; set; }
        private float U { get; set; }
        private float V { get; set; }

        private List<float> Vertices { get; }
        private List<float> Colors { get; }
        private List<float> UVs { get; }

        private static Matrix4x4f GetProjectionMatrix()
        {
            return Matrix4x4f.Ortho(0, Overlay.Width, 0, Overlay.Height, -256, 256);
        }

        public void Draw()
        {
            var bufferRenderer = BufferRenderer.Instance;

            bufferRenderer.Bind();
            bufferRenderer.BindBuffer();
            bufferRenderer.WriteBuffer(GetBuffer());
            Shader.UniformMatrix4F("u_projection", GetProjectionMatrix());

            switch (VertexMode)
            {
                case VertexModes.PositionColor:
                    AddPositionAttribute(0, new IntPtr(0));
                    AddColorAttribute(1, new IntPtr(Vertices.Count * 4));
                    break;
                case VertexModes.PositionColorTexture:
                    AddPositionAttribute(0, new IntPtr(0));
                    AddColorAttribute(1, new IntPtr(Vertices.Count * 4));
                    AddUVsAttribute(2, new IntPtr((Vertices.Count + Colors.Count) * 4));
                    BindTexture();
                    break;
                case VertexModes.PositionTexture:
                    AddPositionAttribute(0, new IntPtr(0));
                    AddUVsAttribute(1, new IntPtr(Vertices.Count * 4));
                    BindTexture();
                    break;
            }

            bufferRenderer.UnbindBuffer();
            Shader.Bind();
            bufferRenderer.Draw(DrawMode, Vertices.Count / 3);
            Shader.Unbind();
            bufferRenderer.Unbind();
        }

        private static void AddPositionAttribute(uint index, IntPtr pointer)
        {
            Gl.VertexAttribPointer(index, 3, VertexAttribType.Float, false, 0, pointer);
            Gl.EnableVertexAttribArray(index);
        }

        private static void AddColorAttribute(uint index, IntPtr pointer)
        {
            Gl.VertexAttribPointer(index, 4, VertexAttribType.Float, false, 0, pointer);
            Gl.EnableVertexAttribArray(index);
        }

        private static void AddUVsAttribute(uint index, IntPtr pointer)
        {
            Gl.VertexAttribPointer(index, 2, VertexAttribType.Float, false, 0, pointer);
            Gl.EnableVertexAttribArray(index);
        }

        private void BindTexture()
        {
            Gl.ActiveTexture(TextureUnit.Texture0);
            Gl.BindTexture(TextureTarget.Texture2d, TextureId);
            Shader.Uniform1I("u_sampler", 0);
        }

        private float[] GetBuffer()
        {
            var floats = new List<float>();
            floats.AddRange(Vertices);
            floats.AddRange(Colors);
            floats.AddRange(UVs);
            return floats.ToArray();
        }

        public BufferBuilder Vert(float x, float y)
        {
            X = x;
            Y = y;
            PositionDone = true;
            return this;
        }

        public BufferBuilder Color(Color color)
        {
            R = color.R / 255f;
            G = color.G / 255f;
            B = color.B / 255f;
            A = color.A / 255f;
            ColorDone = true;
            return this;
        }

        public BufferBuilder UV(float u, float v)
        {
            U = u;
            V = v;
            TextureDone = true;
            return this;
        }

        private bool AddPosition()
        {
            if (!PositionDone) return false;
            Vertices.Add(X);
            Vertices.Add(Y);
            Vertices.Add(1f);
            PositionDone = false;
            return true;
        }

        private bool AddColor()
        {
            if (!ColorDone) return false;
            Colors.Add(R);
            Colors.Add(G);
            Colors.Add(B);
            Colors.Add(A);
            ColorDone = false;
            return true;
        }

        private bool AddUVs()
        {
            if (!TextureDone) return false;
            UVs.Add(U);
            UVs.Add(V);
            TextureDone = false;
            return true;
        }

        public void End()
        {
            switch (VertexMode)
            {
                case VertexModes.PositionColor:
                    if (!(AddPosition() && AddColor())) throw new InvalidVertexException(VertexMode);
                    break;
                case VertexModes.PositionColorTexture:
                    if (!(AddPosition() && AddColor() && AddUVs())) throw new InvalidVertexException(VertexMode);
                    break;
                case VertexModes.PositionTexture:
                    if (!(AddPosition() && AddUVs())) throw new InvalidVertexException(VertexMode);
                    break;
            }
        }

        public void SetTexture(uint texture)
        {
            TextureId = texture;
        }
    }
}