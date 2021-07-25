namespace Enigma.Graphics
{
    public class Pipeline
    {
        public readonly IShader[] Shaders;
        public readonly VertexElement[] VertexElements;
        public readonly ResourceLayout[] Resources;
        public readonly PrimitiveTopology Topology = PrimitiveTopology.TriangleList;
        public readonly PolygonFillMode FillMode = PolygonFillMode.Solid;

        public Pipeline(IShader[] shaders, VertexElement[] vertexElements, params ResourceLayout[] resources)
        {
            Shaders = shaders;
            VertexElements = vertexElements;
            Resources = resources;
        }

        public Pipeline(PrimitiveTopology topology, PolygonFillMode fillMode, IShader[] shaders, VertexElement[] vertexElements, params ResourceLayout[] resources)
        {
            Shaders = shaders;
            VertexElements = vertexElements;
            Resources = resources;
            Topology = topology;
            FillMode = fillMode;
        }
    }
}
