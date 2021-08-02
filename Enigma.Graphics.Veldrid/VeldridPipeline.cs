using System;
using Veldrid;

namespace Enigma.Graphics.Veldrid
{
    public class VeldridPipeline : Pipeline
    {
        public readonly global::Veldrid.Pipeline VdPipeline;

        public VeldridPipeline(ResourceFactory factory, IShader[] shaders, VertexElement[] vertexElements, params ResourceLayout[] resources) : base(shaders, vertexElements, resources)
        {
            GraphicsPipelineDescription gpd = new GraphicsPipelineDescription();
            VdPipeline = factory.CreateGraphicsPipeline(ref gpd);
        }

        public VeldridPipeline(ResourceFactory factory, PrimitiveTopology topology, PolygonFillMode fillMode, IShader[] shaders, VertexElement[] vertexElements, params ResourceLayout[] resources) : base(topology, fillMode, shaders, vertexElements, resources)
        {
            GraphicsPipelineDescription gpd = new GraphicsPipelineDescription();
            gpd.PrimitiveTopology = VeldridUtil.FromEnigmaPrimitive(topology);
            VdPipeline = factory.CreateGraphicsPipeline(ref gpd);
        }
    }
}
