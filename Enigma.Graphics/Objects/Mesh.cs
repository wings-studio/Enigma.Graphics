using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Veldrid;
using Veldrid.Utilities;

namespace Enigma.Graphics.Objects
{
    public class Mesh : CullRenderable
    {
        public override BoundingBox BoundingBox => BoundingBox.Transform(sourceBoundingBox, Transform.GetTransformMatrix());
        public override RenderPasses RenderPasses => RenderPasses.AllShadowMap;
        public string Name { private set; get; }
        public bool DisableShadows { get; set; } = false;
        public bool DisableShadowDepth { get; set; } = false;
        public bool DisableReflection { get; set; } = false;
        public Transform Transform { set; get; }

        private readonly MeshData mesh;
        private readonly BoundingBox sourceBoundingBox;

        private int indexCount;
        private DeviceBuffer vertexBuffer, indexBuffer;
        private Pipeline pipeline;

        public Mesh(string name, MeshData data)
        {
            Name = name;
            mesh = data;
            sourceBoundingBox = data.GetBoundingBox();
            Transform = new Transform();
        }

        public override void CreateDeviceObjects(GraphicsDevice gd, CommandList cl, SceneContext sc)
        {
            ResourceFactory factory = new DisposeCollectorResourceFactory(gd.ResourceFactory);
            vertexBuffer = mesh.CreateVertexBuffer(factory, cl);
            vertexBuffer.Name = Name + "_VB";
            indexBuffer = mesh.CreateIndexBuffer(factory, cl, out indexCount);
            indexBuffer.Name = Name + "_IB";
            (Shader vs, Shader fs) = StaticResourceCache.GetShaders(gd, factory, "ShadowMain");
        }

        public override void Dispose()
        {
            vertexBuffer.Dispose();
            indexBuffer.Dispose();
            pipeline.Dispose();
        }

        public override RenderOrderKey GetRenderOrderKey(Vector3 cameraPosition)
        {
            throw new NotImplementedException();
        }

        public override void Render(GraphicsDevice gd, CommandList cl, SceneContext sc, RenderPasses renderPass)
        {
        }

        public override void UpdatePerFrameResources(GraphicsDevice gd, CommandList cl, SceneContext sc)
        {
            throw new NotImplementedException();
        }
    }
}
