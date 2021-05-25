using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Veldrid;
using Veldrid.Utilities;

namespace Enigma.Graphics.Objects
{
    public abstract class Mesh : RenderObject
    {
        public Transform Transform { get; set; } = new Transform();
        public override BoundingBox BoundingBox => mesh.GetBoundingBox();

        protected IGraphicsStorage Storage => Renderer.Storage;
        protected readonly MeshData mesh;
        protected DeviceBuffer indexBuffer, vertexBuffer;
        protected int indexCount;

        private readonly DisposeCollector collector;

        public Mesh(MeshData data)
        {
            DisposeCollectorResourceFactory f = new (GraphicsDevice.ResourceFactory);
            collector = f.DisposeCollector;
            factory = f;
            mesh = data;
        }

        public override void CreateDeviceObjects()
        {
            vertexBuffer = mesh.CreateVertexBuffer(factory, CommandList);
            indexBuffer = mesh.CreateIndexBuffer(factory, CommandList, out indexCount);
        }

        public override void Dispose()
        {
            collector.DisposeAll();
        }
    }
}
