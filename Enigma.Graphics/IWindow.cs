using System;
using System.Collections.Generic;
using Veldrid;

namespace Enigma.Graphics
{
    public interface IWindow
    {
        string Title { set; get; }
        WindowState State { set; get; }
        bool MouseVisible { set; get; }

        bool Exists { get; }

        event Action OnResized, OnClosing, OnClosed;

        GraphicsDevice CreateGraphicsDevice(GraphicsDeviceOptions options, GraphicsBackend backend);

        void Close();

        void Update();
    }
}
