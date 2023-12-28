using System;

namespace HijackOverlay.Render.Shader
{
    public class ShaderCompileException : Exception
    {
        public ShaderCompileException(string error) : base($"Error Compiling Shader:\n{error}")
        {
        }
    }
}