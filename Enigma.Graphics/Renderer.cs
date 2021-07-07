using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Veldrid;

namespace Enigma.Graphics
{
    public class Renderer : IDisposable
    {
        public static IGraphicsStorage Storage { get; set; }

        public RgbaFloat ClearColor 
        { 
            set 
            {
                foreach (Scene s in renderStages.Values)
                    s.ClearColor = value;
                clearColor = value;
            }
            get => clearColor;
        }
        public IWindow Window { get; protected set; }
        public GraphicsDevice GraphicsDevice { get; protected set; }
        public Scene CurrentScene { get; protected set; }

        public Renderer(IWindow window, bool debug = false)
            : this(window, Veldrid.StartupUtilities.VeldridStartup.GetPlatformDefaultBackend(), debug) { }


        private readonly Dictionary<string, Scene> renderStages = new Dictionary<string, Scene>();
        private RgbaFloat clearColor = RgbaFloat.Black;

        public Renderer(IWindow window, GraphicsBackend backend, bool debug = false)
        {
            Window = window;
            GraphicsDeviceOptions gdo = new GraphicsDeviceOptions(debug);
            GraphicsDevice = window.CreateGraphicsDevice(gdo, backend);
        }

        /// <summary>
        /// Adds <paramref name="renderable"/> to <paramref name="stage"/>
        /// </summary>
        public void Add(string stage, IRenderable renderable) => renderStages[stage].Add(renderable);

        /// <summary>
        /// Adds <paramref name="renderable"/> to given stages
        /// </summary>
        /// <param name="stages">Stages to add renderable</param>
        /// <exception cref="IndexOutOfRangeException">When <paramref name="stages"/> is empty</exception>
        public void Add(IRenderable renderable, params string[] stages)
        {
            if (stages.Length == 0)
                throw new IndexOutOfRangeException("Stages not given for Renderer.Add method");

            foreach (string stage in stages)
                Add(stage, renderable);
        }

        public void Init()
        {
            foreach (Scene s in renderStages.Values)
                s.Init();
        }

        public void Finish()
        {
            Dispose();
            GraphicsDevice.SwapBuffers();
            GraphicsDevice.WaitForIdle();
        }

        public void AddRenderStage<T>(string name, T scene) where T : Scene
        {
            scene.GraphicsDevice = GraphicsDevice;
            scene.Window = Window;
            scene.ClearColor = ClearColor;
            renderStages.Add(name, scene); 
        }

        public void AddRenderStage<T>(string name) where T : Scene, new()
        {
            AddRenderStage(name, new T());
        }

        public void RenderMultiThreading(float deltaSeconds)
        {
            List<Task> stages = new List<Task>();
            foreach (string stage in renderStages.Keys)
            {
                Task task = new Task(() => Render(stage, deltaSeconds));
                task.Start();
                stages.Add(task);
            }
            Task.WaitAll(stages.ToArray());
            Finish();
        }

        public void RenderAll(float deltaSeconds)
        {
            foreach (string stage in renderStages.Keys)
            {
                Render(stage, deltaSeconds);
            }
            Finish();
        }

        /// <summary>
        /// Renders <see cref="Scene"/>
        /// </summary>
        public void Render(string stage, float deltaSeconds)
        {
            Scene scene = renderStages[stage];
            CurrentScene = scene;
            scene.Draw(deltaSeconds);
        }

        public void Dispose()
        {
            foreach (Scene stage in renderStages.Values)
            {
                stage.Dispose();
            }
        }
    }
}
