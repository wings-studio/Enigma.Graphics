using System;
using System.Collections.Generic;

namespace Enigma.Graphics
{
    public interface IRenderTask
    {
        void Render(IGraphicsDevice gd, Scene scene);
        void CreateResources(IGraphicsDevice gd, Scene scene);
    }
}
