using System;
using System.Collections.Generic;
using Veldrid;

namespace Enigma.Graphics
{
    public interface IWindow : IUpdateable
    {
        string Title { set; get; }
        WindowState State { set; get; }
        int Width { set; get; }
        int Height { set; get; }

        bool Exists { get; }

        event Action OnResized, OnClosing, OnClosed;

        GraphicsDevice CreateGraphicsDevice(GraphicsDeviceOptions options, GraphicsBackend backend);

        void Close();
    }
}
