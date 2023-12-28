namespace HijackOverlay.Render.Shader
{
    public class ShaderManager
    {
        private ShaderManager()
        {
            PositionColorShader = new GlShader(Shaders.PositionColorVertex, Shaders.PositionColorFragment);
            PositionColorTextureShader = new GlShader(Shaders.PositionColorTextureVertex, Shaders.PositionColorTextureFragment);
            PositionTextureShader = new GlShader(Shaders.PositionTextureVertex, Shaders.PositionTextureFragment);
        }

        public static ShaderManager Instance { get; } = new();

        public GlShader PositionColorShader { get; }
        public GlShader PositionColorTextureShader { get; }
        public GlShader PositionTextureShader { get; }

        public void Delete()
        {
            PositionColorShader.Delete();
            PositionColorTextureShader.Delete();
            PositionTextureShader.Delete();
        }
    }
}