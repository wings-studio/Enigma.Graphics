﻿using System;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;
using Veldrid;
using System.Numerics;
using Mallos.Input;
using Enigma.Graphics;

namespace Enigma.Sdl
{
    public class SdlWindow : IWindow
    {
        public string Title { get => window.Title; set => window.Title = value; }
        public WindowState State { get => window.WindowState; set => window.WindowState = value; }
        public bool MouseVisible { get => window.CursorVisible; set => window.CursorVisible = value; }
        public bool Exists => window.Exists;

        public int Width { get => window.Width; set => window.Width = value; }
        public int Height { get => window.Height; set => window.Height = value; }
        public Vector2 MouseDelta => window.MouseDelta;
        public VeldridDeviceSet Input { get; private set; }

        public event Action OnResized { add => window.Resized += value; remove => window.Resized -= value; }
        public event Action OnClosing { add => window.Closing += value; remove => window.Closing -= value; }
        public event Action OnClosed { add => window.Closed += value; remove => window.Closed -= value; }

        private readonly Sdl2Window window;

        public SdlWindow(WindowCreateInfo wci)
        {
            window = VeldridStartup.CreateWindow(wci);
            Input = new VeldridDeviceSet();
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

        public virtual void Update(float deltaTime)
        {
            Input.UpdateSnapshot(window.PumpEvents());
            Input.Update(deltaTime);
        }

        public void SetMousePosition(Vector2 position)
        {
            window.SetMousePosition(position);
        }
    }
}
