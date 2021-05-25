using System;
using System.Numerics;
using Veldrid;
using Veldrid.Utilities;

namespace Enigma.Graphics
{
    public interface IRenderable : IDisposable
    {
        GraphicsDevice GraphicsDevice { get; set; }
        CommandList CommandList { get; set; }

        void UpdatePerFrameResources();
        void Render();
        void CreateDeviceObjects();
    }
}
