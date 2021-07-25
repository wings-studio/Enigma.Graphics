namespace Enigma.Graphics
{
    public class Pipeline
    {
        public IShader[] shaders;
        public VertexElement[] vertexElements;
        public ResourceLayout[] resources;
        public PrimitiveTopology topology = PrimitiveTopology.TriangleList;
        public PolygonFillMode fillMode = PolygonFillMode.Solid;

        public Pipeline(IShader[] shaders, VertexElement[] vertexElements, params ResourceLayout[] resources)
        {
            this.shaders = shaders;
            this.vertexElements = vertexElements;
            this.resources = resources;
        }

        public Pipeline(PrimitiveTopology topology, PolygonFillMode fillMode, IShader[] shaders, VertexElement[] vertexElements, params ResourceLayout[] resources)
        {
            this.shaders = shaders;
            this.vertexElements = vertexElements;
            this.resources = resources;
            this.topology = topology;
            this.fillMode = fillMode;
        }
    }
}
