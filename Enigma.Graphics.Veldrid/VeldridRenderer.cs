using System;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace Enigma.Graphics.Veldrid
{
    public class VeldridRenderer : Renderer
    {
        public VeldridRenderer()
        {
            VeldridStartup.CreateWindowAndGraphicsDevice(new WindowCreateInfo(), out Sdl2Window window, out GraphicsDevice gd);
        }
    }
}
