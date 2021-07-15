using System;
using Silk.NET.OpenGL;

namespace Enigma.Graphics.Silk
{
    public class SilkRenderer : Renderer
    {
        public SilkRenderer()
        {
            GraphicsDevice = new GlDevice();
        }
    }
}
