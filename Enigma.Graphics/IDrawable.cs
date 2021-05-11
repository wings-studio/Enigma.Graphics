using System;
using Veldrid;

namespace Enigma.Graphics
{
    public interface IDrawable : IDisposable
    {
        void Draw(CommandList commandList);

        void CreateResources(GraphicsDevice graphicsDevice);
    }
}
