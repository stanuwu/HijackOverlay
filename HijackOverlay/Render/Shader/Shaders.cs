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

        public const string RoundedPositionColorFragment = @"
#version 330

in vec4 vertexColor;

uniform float u_radius;
uniform vec2 u_size;
uniform vec2 u_position;

out vec4 fragmentColor;

float distanceFromCentre(vec2 centre, vec2 size, float radius) {
    return length(max(abs(centre)-size+radius, 0.0f))-radius;
}

void main() {
    if (vertexColor.a == 0.0f) {
        discard;
    }
    float distance = distanceFromCentre(gl_FragCoord.xy - u_position - u_size/2.0f, u_size/2.0f, u_radius);
    float smoothedAlpha = 1.0f-smoothstep(0.0f, 2.0f, distance);
    fragmentColor = vec4(vertexColor.rgb, vertexColor.a * smoothedAlpha);
}
";

        public const string RoundedOutlinePositionColorFragment = @"
#version 330

in vec4 vertexColor;

uniform float u_radius;
uniform float u_thickness;
uniform vec2 u_size;
uniform vec2 u_position;

out vec4 fragmentColor;

float distanceFromCentre(vec2 centre, vec2 size, float radius) {
    return length(max(abs(centre)-size+radius, 0.0f))-radius;
}

void main() {
    if (vertexColor.a == 0.0f) {
        discard;
    }
    float distance = distanceFromCentre(gl_FragCoord.xy - u_position - u_size/2.0f, u_size/2.0f, u_radius);
    float smoothedAlpha = 1.0f-smoothstep(0.0f, 2.0f, distance);
    float borderAlpha = 1.0f-smoothstep(u_thickness - 2.0f, u_thickness, abs(distance));
    vec4 fill = vec4(0.0, 0.0, 0.0, 0.0); 
    fragmentColor = mix(fill, mix(fill, vertexColor, borderAlpha), smoothedAlpha);
}
";
    }
}