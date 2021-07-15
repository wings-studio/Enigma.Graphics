namespace Enigma.Graphics
{
    public class Pipeline
    {
        public IShader[] shaders;
        public ResourceLayout[] resources;
        public PrimitiveTopology topology = PrimitiveTopology.TriangleList;
        public PolygonFillMode fillMode = PolygonFillMode.Solid;

        public Pipeline(IShader[] shaders, params ResourceLayout[] resources)
        {
            this.shaders = shaders;
            this.resources = resources;
        }

        public Pipeline(PrimitiveTopology topology, PolygonFillMode fillMode, IShader[] shaders, params ResourceLayout[] resources)
        {
            this.shaders = shaders;
            this.resources = resources;
            this.topology = topology;
            this.fillMode = fillMode;
        }
    }
}
