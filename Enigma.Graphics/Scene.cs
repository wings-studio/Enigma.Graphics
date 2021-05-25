using System;
using System.Collections;
using System.Collections.Generic;
using Veldrid;

namespace Enigma.Graphics
{
    public class Scene : IEnumerable<IRenderable>, IDisposable
    {
        public GraphicsDevice GraphicsDevice { set { gd = value; cl = Factory.CreateCommandList(); } get => gd; }
        public IWindow Window { get; set; }

        protected ResourceFactory Factory => gd.ResourceFactory;

        protected readonly List<IRenderable> renderables;
        protected CommandList cl;
        private GraphicsDevice gd;

        public Scene()
        {
            renderables = new List<IRenderable>();
        }

        public Scene(GraphicsDevice gd) : this()
        {
            GraphicsDevice = gd;
        }

        public Scene(GraphicsDevice gd, IWindow window) : this(gd)
        {
            Window = window;
        }

        public virtual void Add(IRenderable renderable)
        {
            renderable.GraphicsDevice = GraphicsDevice;
            renderables.Add(renderable);
        }

        public virtual void CreateResources()
        {
            foreach (IRenderable r in renderables)
            {
                r.CreateDeviceObjects();
            }
        }

        public virtual void BeginDraw()
        {
            cl.Begin();
        }

        /// <summary>
        /// Render renderables which was added to this scene
        /// </summary>
        public virtual void Render()
        {
            foreach (IRenderable r in renderables)
            {
                r.CommandList = cl;
                r.Render();
            }
        }

        public virtual void UpdateResources()
        {
            CommandList rescl = Factory.CreateCommandList();
            rescl.Begin();
            foreach (IRenderable r in renderables)
            {
                r.CommandList = rescl;
                r.UpdatePerFrameResources();
            }
            rescl.End();
            GraphicsDevice.SubmitCommands(rescl);
        }

        public virtual void EndDraw()
        {
            cl.End();
            GraphicsDevice.SubmitCommands(cl);
        }

        public virtual void Draw(float deltaSeconds)
        {
            BeginDraw();
            Render();
            EndDraw();
            UpdateResources();
        }

        public IEnumerator<IRenderable> GetEnumerator()
        {
            return renderables.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return renderables.GetEnumerator();
        }

        public virtual void Dispose()
        {
            foreach (IRenderable r in renderables)
            {
                r.Dispose();
            }
        }
    }
}
