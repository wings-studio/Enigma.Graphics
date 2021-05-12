using System;
using System.Collections.Generic;
using Veldrid;

namespace Enigma.Graphics.Sdl
{
    public class SdlApplication : Application
    {
        public SdlApplication(GraphicsBackend backend = GraphicsBackend.Vulkan) : base(backend) { }

        protected override void InitWindow()
        {
            Window = new SdlWindow();
        }
    }
}
