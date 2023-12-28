using System;
using System.Drawing;
using HijackOverlay.Util;
using OpenGL;

namespace HijackOverlay.Render.Texture
{
    public class GlTexture
    {
        public GlTexture(Bitmap image)
        {
            Id = Gl.GenTexture();
            Gl.BindTexture(TextureTarget.Texture2d, Id);
            Gl.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMinFilter, Gl.LINEAR);
            Gl.TexParameteri(TextureTarget.Texture2d, TextureParameterName.TextureMagFilter, Gl.LINEAR);
            Gl.TexImage2D(TextureTarget.Texture2d, Gl.ZERO, InternalFormat.Rgba, image.Width, image.Height, Gl.ZERO, PixelFormat.Rgba, PixelType.UnsignedByte,
                TextureUtil.MakeBuffer(image));
        }

        public GlTexture(string path) : this((Bitmap)Image.FromFile(path))
        {
        }

        public GlTexture(IntPtr hInstance, string name) : this(Bitmap.FromResource(hInstance, name))
        {
        }

        public uint Id { get; }
    }
}