using Veldrid;
using Veldrid.Utilities;

namespace Enigma.Graphics.Objects
{
    public abstract class Mesh : RenderObject
    {
        public Transform Transform { get; set; } = new Transform();
        public override BoundingBox BoundingBox => mesh.GetBoundingBox();

        protected IGraphicsStorage Storage => Renderer.Storage;
        protected readonly IMeshData mesh;
        protected DeviceBuffer indexBuffer, vertexBuffer;
        protected int indexCount;
        protected readonly uint sizeofVertex;

        private DisposeCollector collector;

        public Mesh(IMeshData data)
        {   
            mesh = data;
        }

        public override void CreateDeviceObjects(GraphicsDevice gd, CommandList cl)
        {
            DisposeCollectorResourceFactory drf = new (gd.ResourceFactory);
            factory = drf;
            collector = drf.DisposeCollector;
            vertexBuffer = mesh.CreateVertexBuffer(factory, cl);
            indexBuffer = mesh.CreateIndexBuffer(factory, cl, out indexCount);
        }

        public override void Render(CommandList cl)
        {
            cl.SetPipeline(pipeline);
            cl.SetIndexBuffer(indexBuffer, IndexFormat.UInt16);
            cl.SetVertexBuffer(0, vertexBuffer);
            SetGraphicsSet(cl);
            cl.DrawIndexed((uint)indexCount, 1, 0, 0, 0);
        }

        protected abstract void SetGraphicsSet(CommandList cl);

        public override void Dispose()
        {
            collector.DisposeAll();
        }
    }
}
