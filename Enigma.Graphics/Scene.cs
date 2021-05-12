using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace Enigma.Graphics
{
    public class Scene : IDisposable
    {
        public Camera Camera { set; get; }
        public GraphicsDevice GraphicsDevice { protected set; get; }
        public CommandList CommandList { protected set; get; }
        public RgbaFloat ClearColor { set; get; } = RgbaFloat.Black;

        private readonly List<IDrawable> drawables;

        public Scene(IWindow window, GraphicsBackend backend)
        {
            GraphicsDeviceOptions options = new GraphicsDeviceOptions
            {
                PreferStandardClipSpaceYDirection = true,
                PreferDepthRangeZeroToOne = true
            };
            GraphicsDevice = window.CreateGraphicsDevice(options, backend);

            CommandList = GraphicsDevice.ResourceFactory.CreateCommandList();

            Camera = new Camera(GraphicsDevice, window);

            drawables = new List<IDrawable>();
        }

        public void AddDrawable(IDrawable drawable)
        {
            drawable.CreateResources(GraphicsDevice);
            drawables.Add(drawable);
        }

        public virtual void BeginDraw()
        {
            CommandList.Begin();

            CommandList.SetFramebuffer(GraphicsDevice.SwapchainFramebuffer);
            CommandList.ClearColorTarget(0, ClearColor);
        }

        public virtual void Draw()
        {
            foreach (IDrawable drawable in drawables)
                drawable.Draw(CommandList);
        }

        public virtual void EndDraw()
        {
            CommandList.End();
            GraphicsDevice.SubmitCommands(CommandList);

            GraphicsDevice.SwapBuffers();
        }

        public void Dispose()
        {
            foreach (IDrawable drawable in drawables)
                drawable.Dispose();

            CommandList.Dispose();
            GraphicsDevice.Dispose();
        }
    }
}
