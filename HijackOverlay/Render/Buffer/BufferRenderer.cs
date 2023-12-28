using OpenGL;

namespace HijackOverlay.Render.Buffer
{
    public class BufferRenderer
    {
        private const uint FloatSize = 4;

        private readonly uint _vao;
        private readonly uint _vbo;

        private uint _prevVao;

        private BufferRenderer()
        {
            _vao = Gl.GenVertexArray();
            _vbo = Gl.GenBuffer();
        }

        public static BufferRenderer Instance { get; } = new();

        public void Delete()
        {
            Gl.DeleteBuffers(_vbo);
            Gl.DeleteVertexArrays(_vao);
        }

        public void BindBuffer()
        {
            Gl.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
        }

        public void UnbindBuffer()
        {
            Gl.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void WriteBuffer(float[] data)
        {
            Gl.BufferData(BufferTarget.ArrayBuffer, FloatSize * (uint)data.Length, data, BufferUsage.StaticDraw);
        }

        public void Draw(PrimitiveType drawMode, int verts)
        {
            Gl.DrawArrays(drawMode, 0, verts);
        }

        public void Bind()
        {
            Gl.GetInteger(GetPName.VertexArrayBinding, out _prevVao);
            Gl.BindVertexArray(_vao);
        }

        public void Unbind()
        {
            Gl.BindVertexArray(_prevVao);
        }
    }
}