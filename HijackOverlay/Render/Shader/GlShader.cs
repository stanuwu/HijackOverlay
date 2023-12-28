using System.Text;
using OpenGL;

namespace HijackOverlay.Render.Shader
{
    public class GlShader
    {
        public GlShader(string vertex, string fragment)
        {
            Vertex = LoadShaderProgram(vertex, ShaderType.VertexShader);
            Fragment = LoadShaderProgram(fragment, ShaderType.FragmentShader);
            Id = Gl.CreateProgram();
            Gl.AttachShader(Id, Vertex);
            Gl.AttachShader(Id, Fragment);
            Gl.LinkProgram(Id);
        }

        public uint Id { get; }
        public uint Vertex { get; }
        public uint Fragment { get; }

        public void Delete()
        {
            Gl.DetachShader(Id, Vertex);
            Gl.DetachShader(Id, Fragment);
            Gl.DeleteProgram(Id);
            Gl.DeleteShader(Vertex);
            Gl.DeleteShader(Fragment);
        }

        public void Bind()
        {
            Gl.UseProgram(Id);
        }

        public void Unbind()
        {
            Gl.UseProgram(0);
        }

        public void Uniform1I(string name, int value)
        {
            Bind();
            Gl.Uniform1(Gl.GetUniformLocation(Id, name), value);
            Unbind();
        }

        public void Uniform1F(string name, float value)
        {
            Bind();
            Gl.Uniform1(Gl.GetUniformLocation(Id, name), value);
            Unbind();
        }

        public void Uniform2F(string name, float value1, float value2)
        {
            Bind();
            Gl.Uniform2(Gl.GetUniformLocation(Id, name), value1, value2);
            Unbind();
        }

        public void UniformMatrix4F(string name, Matrix4x4f value)
        {
            Bind();
            Gl.UniformMatrix4f(Gl.GetUniformLocation(Id, name), 1, false, value);
            Unbind();
        }


        private static uint LoadShaderProgram(string shader, ShaderType shaderType)
        {
            var i = Gl.CreateShader(shaderType);
            Gl.ShaderSource(i, new[] { shader });
            Gl.CompileShader(i);
            Gl.GetShader(i, ShaderParameterName.CompileStatus, out var isCompiled);
            if (isCompiled == Gl.TRUE) return i;
            Gl.GetShader(i, ShaderParameterName.InfoLogLength, out var maxLength);
            var error = new StringBuilder();
            Gl.GetShaderInfoLog(i, maxLength, out maxLength, error);
            throw new ShaderCompileException(error.ToString());
        }
    }
}