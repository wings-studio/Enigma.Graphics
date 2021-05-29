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

        public abstract void CreateDeviceObjects(GraphicsDevice gd, CommandList cl);

        public abstract void Dispose();

        public abstract void Render(CommandList cl);

        public abstract void UpdatePerFrameResources(CommandList cl);
    }
}
