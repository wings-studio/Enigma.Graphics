using System;
using Veldrid;

namespace Enigma.Graphics.Veldrid
{
    public class VeldridPipeline : Pipeline
    {
        public global::Veldrid.Pipeline VdPipeline;

        public VeldridPipeline(IShader[] shaders, VertexElement[] vertexElements, params ResourceLayout[] resources) : base(shaders, vertexElements, resources)
        {
        }

        public VeldridPipeline(PrimitiveTopology topology, PolygonFillMode fillMode, IShader[] shaders, VertexElement[] vertexElements, params ResourceLayout[] resources) : base(topology, fillMode, shaders, vertexElements, resources)
        {
        }
    }
}
