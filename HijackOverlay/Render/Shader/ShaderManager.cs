namespace HijackOverlay.Render.Shader
{
    public class ShaderManager
    {
        private ShaderManager()
        {
            PositionColorShader = new GlShader(Shaders.PositionColorVertex, Shaders.PositionColorFragment);
            PositionColorTextureShader = new GlShader(Shaders.PositionColorTextureVertex, Shaders.PositionColorTextureFragment);
            PositionTextureShader = new GlShader(Shaders.PositionTextureVertex, Shaders.PositionTextureFragment);
            RoundedPositionColorShader = new GlShader(Shaders.PositionColorVertex, Shaders.RoundedPositionColorFragment);
            RoundedOutlinePositionColorShader = new GlShader(Shaders.PositionColorVertex, Shaders.RoundedOutlinePositionColorFragment);
        }

        public static ShaderManager Instance { get; } = new();

        public GlShader PositionColorShader { get; }
        public GlShader PositionColorTextureShader { get; }
        public GlShader PositionTextureShader { get; }
        public GlShader RoundedPositionColorShader { get; }
        public GlShader RoundedOutlinePositionColorShader { get; }

        public void Delete()
        {
            PositionColorShader.Delete();
            PositionColorTextureShader.Delete();
            PositionTextureShader.Delete();
            RoundedPositionColorShader.Delete();
            RoundedOutlinePositionColorShader.Delete();
        }
    }
}