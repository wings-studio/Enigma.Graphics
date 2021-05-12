using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Veldrid;

namespace Enigma.Graphics
{
    public abstract class Application : System.IDisposable
    {
        public GraphicsDevice GraphicsDevice { protected set; get; }
        public IWindow Window { protected set; get; }
        public CommandList CommandList { protected set; get; }
        public RgbaFloat ClearColor { set; get; } = RgbaFloat.Black;

        private readonly List<IDrawable> drawables;

        public Application(GraphicsBackend backend = GraphicsBackend.Vulkan)
        {
            InitWindow();

            GraphicsDeviceOptions options = new GraphicsDeviceOptions
            {
                PreferStandardClipSpaceYDirection = true,
                PreferDepthRangeZeroToOne = true
            };
            GraphicsDevice = Window.CreateGraphicsDevice(options, backend);

            drawables = new List<IDrawable>();
        }

        /// <summary>
        /// Create <see cref="Window"/> before creating of <see cref="GraphicsDevice"/>
        /// </summary>
        protected abstract void InitWindow();

        public void AddDrawable(IDrawable drawable)
        {
            drawable.CreateResources(GraphicsDevice);
            drawables.Add(drawable);
        }

        public virtual void Run()
        {
            CommandList = GraphicsDevice.ResourceFactory.CreateCommandList();

            while (Window.Exists)
            {
                Window.Update();

                if (Window.Exists)
                {
                    BeginDraw();
                    Draw();
                    EndDraw();
                }
            }
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

        public void Exit()
        {
            Window?.Close();
        }

        public void Dispose()
        {
            foreach (IDrawable drawable in drawables)
                drawable.Dispose();

            CommandList.Dispose();
            GraphicsDevice.Dispose();
        }

        public Stream OpenResourcesStream(string name) => GetType().Assembly.GetManifestResourceStream(name);

        public byte[] ReadBytesFromResources(string name)
        {
            using (Stream stream = OpenResourcesStream(name))
            {
                byte[] bytes = new byte[stream.Length];
                using (MemoryStream ms = new MemoryStream(bytes))
                {
                    stream.CopyTo(ms);
                    return bytes;
                }
            }
        }

        public T LoadFromResources<T>(string name)
        {
            BinaryFormatter bf = new ();
            return (T)bf.Deserialize(OpenResourcesStream(name));
        }
    }
}
