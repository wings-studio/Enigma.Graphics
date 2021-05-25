using System;
using System.Collections.Generic;
using System.Numerics;
using Veldrid;
using Veldrid.Utilities;

namespace Enigma.Graphics.Objects
{
    public abstract class RenderObject : IRenderable
    {
        public GraphicsDevice GraphicsDevice { get; set; }
        public CommandList CommandList { get; set; }
        public abstract BoundingBox BoundingBox { get; }

        protected ResourceFactory factory;
        protected Pipeline pipeline;

        protected void CreatePipeline(GraphicsPipelineDescription gpd)
        {
            pipeline = Renderer.Storage.GetPipeline(factory, gpd);
        }

        public abstract void CreateDeviceObjects();

        public abstract void Dispose();

        public abstract void Render();

        public abstract void UpdatePerFrameResources();
    }
}
