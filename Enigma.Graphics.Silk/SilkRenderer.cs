using Silk.NET.Windowing;

namespace Enigma.Graphics.Silk
{
    public class SilkRenderer : Renderer
    {
        public IWindow Window;
        public override bool IsRunning 
        { 
            get => base.IsRunning;
            set 
            { 
                base.IsRunning = value;
                if (value) Window.Run();
                else Window.Close();
            }
        }

        public SilkRenderer()
        {
            Window = global::Silk.NET.Windowing.Window.Create(WindowOptions.Default);
            GraphicsDevice = new OpenGL.GlDevice(Window);
            Window.Render += Window_Render;
            Window.Load += CreateResources;
            Window.Closing += () => GraphicsDevice.Dispose();
        }

        private void Window_Render(double deltaTime)
        {
            BeginFrame();
            RenderFrame();
            EndFrame();
        }

        public override void Render()
        {
            IsRunning = true;
        }
    }
}
