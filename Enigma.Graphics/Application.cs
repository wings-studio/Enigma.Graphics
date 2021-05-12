using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Veldrid;

namespace Enigma.Graphics
{
    public abstract class Application : System.IDisposable
    {
        public Scene Scene { protected set; get; }
        public IWindow Window { protected set; get; }

        public Application(GraphicsBackend backend = GraphicsBackend.Vulkan)
        {
            InitWindow();

            Scene = new Scene(Window, backend);
        }

        /// <summary>
        /// Create <see cref="Window"/> before creating of <see cref="GraphicsDevice"/>
        /// </summary>
        protected abstract void InitWindow();

        public void AddDrawable(IDrawable drawable)
            => Scene.AddDrawable(drawable);

        public virtual void Run()
        {
            while (Window.Exists)
            {
                Window.Update();

                if (Window.Exists)
                {
                    Scene.BeginDraw();
                    Scene.Draw();
                    Scene.EndDraw();
                }
            }
        }

        public void Exit()
        {
            Window?.Close();
        }

        public void Dispose()
        {
            Scene.Dispose();
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
