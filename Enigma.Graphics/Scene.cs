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
        public RgbaFloat ClearColor { get; set; }

        protected ResourceFactory Factory => gd.ResourceFactory;

        protected readonly List<IRenderable> renderables;
        protected CommandList cl;
        private GraphicsDevice gd;

        public Scene()
        {
            renderables = new List<IRenderable>();
            ClearColor = RgbaFloat.Black;
        }

        public Scene(GraphicsDevice gd) : this()
        {
            GraphicsDevice = gd;
        }

        public Scene(GraphicsDevice gd, IWindow window) : this(gd)
        {
            Window = window;
        }

        public virtual void Init()
        {
            foreach (IRenderable r in renderables)
                r.CreateDeviceObjects(gd, cl);
        }

        public virtual void Add(IRenderable renderable)
        {
            renderables.Add(renderable);
        }

        public virtual void BeginDraw(CommandList cl)
        {
            cl.Begin();
            cl.SetFramebuffer(GraphicsDevice.SwapchainFramebuffer);
            cl.ClearColorTarget(0, ClearColor);
            //cl.ClearDepthStencil(1f);
        }

        /// <summary>
        /// Render renderables which was added to this scene
        /// </summary>
        public virtual void Render(CommandList cl)
        {
            foreach (IRenderable r in renderables)
            {
                r.Render(cl);
            }
        }

        public virtual void UpdateResources()
        {
            CommandList rescl = Factory.CreateCommandList();
            rescl.Begin();
            foreach (IRenderable r in renderables)
            {
                r.UpdatePerFrameResources(rescl);
            }
            rescl.End();
            GraphicsDevice.SubmitCommands(rescl);
        }

        public virtual void EndDraw(CommandList cl)
        {
            cl.End();
            GraphicsDevice.SubmitCommands(cl);
        }

        public virtual void Draw(float deltaSeconds)
        {
            BeginDraw(cl);
            Render(cl);
            EndDraw(cl);
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
