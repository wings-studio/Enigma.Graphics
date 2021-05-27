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
        protected readonly IMeshData mesh;
        protected DeviceBuffer indexBuffer, vertexBuffer;
        protected int indexCount;
        protected readonly uint sizeofVertex;

        private DisposeCollector collector;

        public Mesh(IMeshData data)
        {   
            mesh = data;
        }

        public override void CreateDeviceObjects()
        {
            DisposeCollectorResourceFactory drf = new (GraphicsDevice.ResourceFactory);
            factory = drf;
            collector = drf.DisposeCollector;
            vertexBuffer = mesh.CreateVertexBuffer(factory, CommandList);
            indexBuffer = mesh.CreateIndexBuffer(factory, CommandList, out indexCount);
        }

        public override void Dispose()
        {
            collector.DisposeAll();
        }
    }
}
