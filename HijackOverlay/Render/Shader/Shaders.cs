namespace HijackOverlay.Render.Shader
{
    public static class Shaders
    {
        public const string PositionColorVertex = @"
#version 330

layout (location = 0) in vec3 i_pos;
layout (location = 1) in vec4 i_color;

uniform mat4 u_projection;

out vec4 vertexColor;

void main() {
    gl_Position = u_projection * vec4(i_pos, 1.0f);
    vertexColor = i_color;
}
";

        public const string PositionColorFragment = @"
#version 330

in vec4 vertexColor;

out vec4 fragmentColor;

void main() {
    if (vertexColor.a == 0.0f) {
        discard;
    }
    fragmentColor = vertexColor;
}
";

        public const string PositionColorTextureVertex = @"
#version 330

layout (location = 0) in vec3 i_pos;
layout (location = 1) in vec4 i_color;
layout (location = 2) in vec2 i_uv;

uniform mat4 u_projection;

out vec4 vertexColor;
out vec2 textureCoord;

void main() {
    gl_Position = u_projection * vec4(i_pos, 1.0f);

    vertexColor = i_color;
    textureCoord = i_uv;
}
";

        public const string PositionColorTextureFragment = @"
#version 330

in vec4 vertexColor;
in vec2 textureCoord;

uniform sampler2D u_sampler;

out vec4 fragmentColor;

void main() {
    vec4 color = texture(u_sampler, textureCoord) * vertexColor;
    if (color.a == 0.0f) {
        discard;
    }
    fragmentColor = color;
}
";

        public const string PositionTextureVertex = @"
#version 330

layout (location = 0) in vec3 i_pos;
layout (location = 1) in vec2 i_uv;

uniform mat4 u_projection;

out vec2 textureCoord;

void main() {
    gl_Position = u_projection * vec4(i_pos, 1.0f);

    textureCoord = i_uv;
}
";

        public const string PositionTextureFragment = @"
#version 330

in vec2 textureCoord;

uniform sampler2D u_sampler;

out vec4 fragmentColor;

void main() {
    vec4 color = texture(u_sampler, textureCoord);
    if (color.a == 0.0f) {
        discard;
    }
    fragmentColor = color;
}
";
    }
}