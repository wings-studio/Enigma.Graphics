using System.Numerics;
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
        protected DeviceBuffer indexBuffer;
        protected DeviceBuffer vertexBuffer;
        protected DeviceBuffer projectionBuffer;
        protected DeviceBuffer viewBuffer;
        protected DeviceBuffer worldBuffer;
        protected int indexCount;

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

            projectionBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));
            viewBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));
            worldBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));
        }

        public override void Render(CommandList cl, Camera camera)
        {
            cl.SetPipeline(pipeline);
            cl.SetIndexBuffer(indexBuffer, IndexFormat.UInt16);
            cl.SetVertexBuffer(0, vertexBuffer);

            //cl.UpdateBuffer(projectionBuffer, 0, camera.ProjectionMatrix);
            //cl.UpdateBuffer(viewBuffer, 0, camera.ViewMatrix);
            cl.UpdateBuffer(projectionBuffer, 0, Matrix4x4.CreatePerspectiveFieldOfView(
                1.0f,
                camera.AspectRatio,
                0.5f,
                100f));
            cl.UpdateBuffer(viewBuffer, 0, camera.ViewMatrix); //Matrix4x4.CreateLookAt(Vector3.UnitZ * 2.5f, Vector3.Zero, Vector3.UnitY));
            cl.UpdateBuffer(worldBuffer, 0, Transform.GetTransformMatrix());

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
