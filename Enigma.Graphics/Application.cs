using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Runtime.Serialization.Formatters.Binary;
using Veldrid;
using Veldrid.ImageSharp;
using Veldrid.Utilities;

namespace Enigma.Graphics
{
    public abstract class Application : System.IDisposable
    {
        public Scene Scene { protected set; get; }
        public IWindow Window { protected set; get; }

        private GraphicsDevice _gd;
        private readonly SceneContext _sc = new SceneContext();
        private bool _windowResized;
        //private RenderOrderKeyComparer _renderOrderKeyComparer = new RenderOrderKeyComparer();
        private bool _recreateWindow = true;

        private static double _desiredFrameLengthSeconds = 1.0 / 60.0;
        private static bool _limitFrameRate = false;
        //private static FrameTimeAverager _fta = new FrameTimeAverager(0.666);
        private CommandList _frameCommands;

        private event Action<int, int> _resizeHandled;

        private readonly string[] _msaaOptions = new string[] { "Off", "2x", "4x", "8x", "16x", "32x" };
        private int _msaaOption = 0;
        private TextureSampleCount? _newSampleCount;

        private readonly Dictionary<string, ImageSharpTexture> _textures = new Dictionary<string, ImageSharpTexture>();

        public Application(GraphicsBackend backend = GraphicsBackend.Vulkan)
        {
            InitWindow();

            GraphicsDeviceOptions gdOptions = new GraphicsDeviceOptions(false, null, false, ResourceBindingModel.Improved, true, true, true);
#if DEBUG
            gdOptions.Debug = true;
#endif
            Scene = new Scene(Window, Window.CreateGraphicsDevice(gdOptions, backend));
        }

        /// <summary>
        /// Create <see cref="Window"/> before creating of <see cref="GraphicsDevice"/>
        /// </summary>
        protected abstract void InitWindow();

        public void AddRenderable(IRenderable renderable)
            => Scene.AddRenderable(renderable);

        public ImageSharpTexture LoadTexture(string texturePath, bool mipmap) // Plz don't call this with the same texturePath and different mipmap values.
        {
            if (!_textures.TryGetValue(texturePath, out ImageSharpTexture tex))
            {
                tex = new ImageSharpTexture(texturePath, mipmap, true);
                _textures.Add(texturePath, tex);
            }

            return tex;
        }

        public void AddTexturedMesh(
            MeshData meshData,
            ImageSharpTexture texData,
            ImageSharpTexture alphaTexData,
            MaterialPropsAndBuffer materialProps,
            Vector3 position,
            Quaternion rotation,
            Vector3 scale,
            string name)
        {
            TexturedMesh mesh = new TexturedMesh(name, meshData, texData, alphaTexData, materialProps);
            mesh.Transform.Position = position;
            mesh.Transform.Rotation = rotation;
            mesh.Transform.Scale = scale;
            AddRenderable(mesh);
        }

        public virtual void Run()
        {
            long previousFrameTicks = 0;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            while (Window.Exists)
            {
                long currentFrameTicks = sw.ElapsedTicks;
                double deltaSeconds = (currentFrameTicks - previousFrameTicks) / (double)Stopwatch.Frequency;

                while (_limitFrameRate && deltaSeconds < _desiredFrameLengthSeconds)
                {
                    currentFrameTicks = sw.ElapsedTicks;
                    deltaSeconds = (currentFrameTicks - previousFrameTicks) / (double)Stopwatch.Frequency;
                }

                previousFrameTicks = currentFrameTicks;

                Window.Update();

                if (!Window.Exists)
                {
                    break;
                }

                Draw();
            }

            sw.Stop();
        }

        private void Draw()
        {
            Debug.Assert(Window.Exists);
            int width = Window.Width;
            int height = Window.Height;

            if (_windowResized)
            {
                _windowResized = false;

                _gd.ResizeMainWindow((uint)width, (uint)height);
                Scene.Camera.WindowResized(width, height);
                _resizeHandled?.Invoke(width, height);
                CommandList cl = _gd.ResourceFactory.CreateCommandList();
                cl.Begin();
                _sc.RecreateWindowSizedResources(_gd, cl);
                cl.End();
                _gd.SubmitCommands(cl);
                cl.Dispose();
            }

            if (_newSampleCount != null)
            {
                _sc.MainSceneSampleCount = _newSampleCount.Value;
                _newSampleCount = null;
                //DestroyAllObjects();
                //CreateAllObjects();
            }

            _frameCommands.Begin();

            Scene.RenderAllStages(_gd, _frameCommands, _sc);
            _gd.SwapBuffers();
        }

        public void Exit()
        {
            Window?.Close();
        }

        public void Dispose()
        {
            Scene.DestroyAllDeviceObjects();
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
