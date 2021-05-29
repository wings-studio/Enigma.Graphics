using System;
using Veldrid;

namespace Enigma.Graphics
{
    public interface IRenderable : IDisposable
    {
        void UpdatePerFrameResources(CommandList cl);
        void Render(CommandList cl);
        void CreateDeviceObjects(GraphicsDevice gd, CommandList cl);
    }
}
