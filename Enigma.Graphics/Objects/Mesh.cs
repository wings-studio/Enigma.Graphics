using Veldrid;
using Veldrid.Utilities;

namespace Enigma.Graphics.Objects
{
    public abstract class Mesh<T> : RenderObject where T : unmanaged, IVertexInfo
    {
        public Transform Transform { get; set; } = new Transform();
        public override BoundingBox BoundingBox => mesh.GetBoundingBox();

        protected IGraphicsStorage Storage => Renderer.Storage;
        protected readonly IMeshData<T> mesh;
        protected DeviceBuffer indexBuffer, vertexBuffer;
        protected int indexCount;
        protected readonly uint sizeofVertex;

        private DisposeCollector collector;

        public Mesh(IMeshData<T> data)
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

        public override void Render(CommandList cl, Camera camera)
        {
            cl.SetPipeline(pipeline);
            cl.SetIndexBuffer(indexBuffer, IndexFormat.UInt16);
            cl.SetVertexBuffer(0, vertexBuffer);
            SetGraphicsSet(cl, camera);
            cl.DrawIndexed((uint)indexCount, 1, 0, 0, 0);
        }

        protected abstract void SetGraphicsSet(CommandList cl, Camera camera);

        public override void Dispose()
        {
            collector.DisposeAll();
        }
    }
}
