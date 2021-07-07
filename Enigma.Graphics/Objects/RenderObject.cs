using System;
using System.Collections.Generic;
using System.Numerics;
using Veldrid;
using Veldrid.Utilities;

namespace Enigma.Graphics.Objects
{
    public abstract class RenderObject : IRenderable
    {
        public abstract BoundingBox BoundingBox { get; }

        protected ResourceFactory factory;
        protected Pipeline pipeline;

        protected void CreatePipeline(GraphicsPipelineDescription gpd)
        {
            pipeline = Renderer.Storage.GetPipeline(factory, gpd);
        }

        [Obsolete("This method throws exception without camera. Use Render(CommandList, Camera)")]
        public void Render(CommandList cl)
        {
            throw new ArgumentException("RenderObject cannot be render without camera");
        }

        public abstract void CreateDeviceObjects(GraphicsDevice gd, CommandList cl);

        public abstract void Dispose();

        public abstract void Render(CommandList cl, Camera camera);

        public abstract void UpdatePerFrameResources(CommandList cl);
    }
}
