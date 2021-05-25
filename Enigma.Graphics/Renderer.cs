using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Veldrid;

namespace Enigma.Graphics
{
    public class Renderer
    {
        public IWindow Window { get; protected set; }
        public GraphicsDevice GraphicsDevice { get; protected set; }
        public Scene CurrentScene { get; protected set; }

        public Renderer(IWindow window, bool debug = false)
            : this(window, Veldrid.StartupUtilities.VeldridStartup.GetPlatformDefaultBackend(), debug) { }


        private readonly Dictionary<string, Scene> renderStages = new Dictionary<string, Scene>();

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

        public void AddRenderStage(string name, Scene scene) 
        {
            scene.GraphicsDevice = GraphicsDevice;
            scene.Window = Window;
            renderStages.Add(name, scene); 
        }

        public async void RenderMultiThreading(float deltaSeconds)
        {
            foreach (string stage in renderStages.Keys)
            {
                await Task.Run(() => Render(stage, deltaSeconds));
            }
        }

        public void RenderAll(float deltaSeconds)
        {
            foreach (string stage in renderStages.Keys)
            {
                Render(stage, deltaSeconds);
            }
        }

        /// <summary>
        /// Renders <see cref="Scene"/>
        /// </summary>
        public void Render(string stage, float deltaSeconds)
        {
            using Scene scene = renderStages[stage];
            CurrentScene = scene;
            scene.CreateResources();
            scene.Draw(deltaSeconds);
        }
    }
}
