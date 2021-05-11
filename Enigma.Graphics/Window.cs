using System;
using System.Collections.Generic;
using Veldrid;

namespace Enigma.Graphics
{
    public abstract class Window
    {
        public string Title { set; get; }
        public WindowState State { set; get; }

        public bool Exists { protected set; get; }

        public event Action OnResized, OnClosing, OnClosed;

        public Window(int X = 0, int Y = 0, int width = 0, int height = 0, string title = "", WindowState initialState = WindowState.Normal) { }

        public abstract GraphicsDevice CreateGraphicsDevice(GraphicsDeviceOptions options, GraphicsBackend backend);
    }
}
