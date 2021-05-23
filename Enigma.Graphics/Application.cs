using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using Veldrid;
using Veldrid.ImageSharp;
using Veldrid.Utilities;

namespace Enigma.Graphics
{
    public abstract class Application : IDisposable
    {
        public Scene Scene { protected set; get; }
        public IWindow Window { protected set; get; }
        public GraphicsDevice GraphicsDevice { protected set; get; }
        /// <summary>
        /// Time to draw one frame in seconds
        /// </summary>
        public float DeltaTime { protected set; get; }
        public static FrameTimeAverager AppTime { protected set; get; }

        protected readonly SceneContext _sc = new ();
        protected bool _windowResized;
        protected RenderOrderKeyComparer _renderOrderKeyComparer = new ();
        protected bool _recreateWindow = true;

        protected static double _desiredFrameLengthSeconds = 1.0 / 60.0;
        protected static bool _limitFrameRate = false;
        protected CommandList _frameCommands;

        protected event Action<int, int> _resizeHandled;

        protected TextureSampleCount? _newSampleCount;

        protected readonly Dictionary<string, ImageSharpTexture> _textures = new ();

        public Application(GraphicsBackend backend = GraphicsBackend.Vulkan)
        {
            AppTime = new FrameTimeAverager(0.660);

            InitWindow();

            GraphicsDeviceOptions gdOptions = new (false, null, false, ResourceBindingModel.Improved, true, true, true);
#if DEBUG
            gdOptions.Debug = true;
#endif
            GraphicsDevice = Window.CreateGraphicsDevice(gdOptions, backend);

            Scene = new Scene(Window, GraphicsDevice);
            _sc.SetCurrentScene(Scene);
        }

        /// <summary>
        /// Create <see cref="Window"/> before creating of <see cref="Veldrid.GraphicsDevice"/>
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

        public void Run()
        {
            long previousFrameTicks = 0;
            Stopwatch sw = new ();
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

                DeltaTime = (float)deltaSeconds;

                previousFrameTicks = currentFrameTicks;

                Window.Update(DeltaTime);

                if (!Window.Exists)
                {
                    break;
                }

                Update();

                Draw();
            }

            sw.Stop();
        }

        /// <summary>
        /// Calls every frame before <see cref="Draw"/>
        /// </summary>
        protected virtual void Update() 
        {
            Scene.Update(DeltaTime);
            AppTime.AddTime(DeltaTime);
        }

        protected void Draw()
        {
#if DEBUG
            Debug.Assert(Window.Exists);
#endif
            int width = Window.Width;
            int height = Window.Height;

            if (_windowResized)
            {
                _windowResized = false;

                GraphicsDevice.ResizeMainWindow((uint)width, (uint)height);
                Scene.Camera.WindowResized(width, height);
                _resizeHandled?.Invoke(width, height);
                CommandList cl = GraphicsDevice.ResourceFactory.CreateCommandList();
                cl.Begin();
                _sc.RecreateWindowSizedResources(GraphicsDevice, cl);
                cl.End();
                GraphicsDevice.SubmitCommands(cl);
                cl.Dispose();
            }

            if (_newSampleCount != null)
            {
                _sc.MainSceneSampleCount = _newSampleCount.Value;
                _newSampleCount = null;
                DestroyAllObjects();
                CreateAllObjects();
            }

            _frameCommands.Begin();

            Scene.RenderAllStages(GraphicsDevice, _frameCommands, _sc);
            GraphicsDevice.SwapBuffers();
        }

        public void Exit()
        {
            Window?.Close();
        }

        public void ChangeBackend(GraphicsBackend backend) => ChangeBackend(backend, false);
        public void ChangeBackend(GraphicsBackend backend, bool forceRecreateWindow)
        {
            DestroyAllObjects();
            bool syncToVBlank = GraphicsDevice.SyncToVerticalBlank;
            GraphicsDevice.Dispose();

            if (_recreateWindow || forceRecreateWindow)
                InitWindow();

            GraphicsDeviceOptions gdOptions = new GraphicsDeviceOptions(false, null, syncToVBlank, ResourceBindingModel.Improved, true, true, true);
#if DEBUG
            gdOptions.Debug = true;
#endif
            GraphicsDevice = Window.CreateGraphicsDevice(gdOptions, backend);

            Scene.Camera.UpdateBackend(GraphicsDevice, Window);

            CreateAllObjects();
        }

        protected abstract void DestroyAllDeviceObjects();

        protected void DestroyAllObjects()
        {
            GraphicsDevice.WaitForIdle();
            _frameCommands.Dispose();
            _sc.DestroyDeviceObjects();
            Scene.DestroyAllDeviceObjects();
            DestroyAllDeviceObjects();
            StaticResourceCache.DestroyAllDeviceObjects();
            GraphicsDevice.WaitForIdle();
        }

        protected abstract void CreateAllDeviceObjects(CommandList cl);

        protected void CreateAllObjects()
        {
            _frameCommands = GraphicsDevice.ResourceFactory.CreateCommandList();
            _frameCommands.Name = "Frame Commands List";
            CommandList initCL = GraphicsDevice.ResourceFactory.CreateCommandList();
            initCL.Name = "Recreation Initialization Command List";
            initCL.Begin();
            _sc.CreateDeviceObjects(GraphicsDevice, initCL, _sc);
            CreateAllDeviceObjects(initCL);
            Scene.CreateAllDeviceObjects(GraphicsDevice, initCL, _sc);
            initCL.End();
            GraphicsDevice.SubmitCommands(initCL);
            initCL.Dispose();
        }

        public void Dispose()
        {
            DestroyAllObjects();
        }
    }
}
