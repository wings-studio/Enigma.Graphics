using System;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;
using Veldrid;
using System.Numerics;

namespace Enigma.Graphics.Sdl
{
    public class SdlWindow : IWindow
    {
        public string Title { get => window.Title; set => window.Title = value; }
        public WindowState State { get => window.WindowState; set => window.WindowState = value; }
        public bool MouseVisible { get => window.CursorVisible; set => window.CursorVisible = value; }
        public bool Exists => window.Exists;

        public int Width { get => window.Width; set => window.Width = value; }
        public int Height { get => window.Height; set => window.Height = value; }
        public Vector2 MouseDelta { get => window.MouseDelta; }

        public event Action OnResized { add => window.Resized += value; remove => window.Resized -= value; }
        public event Action OnClosing { add => window.Closing += value; remove => window.Closing -= value; }
        public event Action OnClosed { add => window.Closed += value; remove => window.Closed -= value; }
        public event Action<KeyEvent> OnKeyDown { add => window.KeyDown += value; remove => window.KeyDown -= value; }
        public event Action<KeyEvent> OnKeyUp { add => window.KeyUp += value; remove => window.KeyUp -= value; }
        public event Action<MouseEvent> OnMouseDown { add => window.MouseDown += value; remove => window.MouseDown -= value; }
        public event Action<MouseEvent> OnMouseUp { add => window.MouseUp += value; remove => window.MouseUp -= value; }
        public event Action<MouseMoveEventArgs> OnMouseMove 
        { 
            add => window.MouseMove += value;
            remove => window.MouseMove -= value;
        }
        public event Action<MouseWheelEventArgs> OnMouseScroll 
        { 
            add => window.MouseWheel += value;
            remove => window.MouseWheel -= value;
        }

        private readonly Sdl2Window window;

        public SdlWindow(WindowCreateInfo wci)
        {
            window = VeldridStartup.CreateWindow(wci);
            Input.Input.Inputable = this;
        }

        public SdlWindow() : this(new WindowCreateInfo() { WindowHeight = 500, WindowWidth = 1000, X = 50, Y = 50 })
        { }

        public void Close()
        {
            window.Close();
        }

        public GraphicsDevice CreateGraphicsDevice(GraphicsDeviceOptions options, GraphicsBackend backend)
        {
            return VeldridStartup.CreateGraphicsDevice(window, options, backend);
        }

        public virtual void Update()
        {
            Input.Input.UpdateFrameInput(window.PumpEvents());
        }

        public void SetMousePosition(Vector2 position)
        {
            window.SetMousePosition(position);
        }
    }
}
