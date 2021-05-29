using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Veldrid;

namespace Enigma.Graphics.Objects
{
    public class TexturedMesh : Mesh
    {
        public string TexturePath { get; set; }

        private ResourceSet _projViewSet;
        private ResourceSet _worldTextureSet;

        public TexturedMesh(IMeshData data) : base(data) 
        {
            mesh.VertexSize = Util.SizeOf<VertexPositionTexture>();
        }

        public override void CreateDeviceObjects(GraphicsDevice gd, CommandList cl)
        {
            base.CreateDeviceObjects(gd, cl);

            DeviceBuffer _projectionBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));
            DeviceBuffer _viewBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));
            DeviceBuffer _worldBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));

            Texture tex;
            try
            {
                tex = Storage.GetTexture2D(gd, factory, new Veldrid.ImageSharp.ImageSharpTexture(TexturePath));
            }
            catch
            {
                tex = Storage.GetColorTexture(gd, factory, RgbaByte.Pink);
            }
            TextureView _surfaceTextureView = Storage.GetTextureView(factory, tex);
            (Shader vs, Shader fs) = Storage.GetShaders(gd, factory, "BaseMesh");
            ShaderSetDescription shaderSet = new ShaderSetDescription(
                new[]
                {
                    new VertexLayoutDescription(
                        new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
                        new VertexElementDescription("TexCoords", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2))
                },
                new[] { vs, fs });

            ResourceLayout projViewLayout = factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("ProjectionBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                    new ResourceLayoutElementDescription("ViewBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex)));

            ResourceLayout worldTextureLayout = factory.CreateResourceLayout(
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("WorldBuffer", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                    new ResourceLayoutElementDescription("SurfaceTexture", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                    new ResourceLayoutElementDescription("SurfaceSampler", ResourceKind.Sampler, ShaderStages.Fragment)));

            CreatePipeline(new GraphicsPipelineDescription(
                BlendStateDescription.SingleOverrideBlend,
                DepthStencilStateDescription.DepthOnlyLessEqual,
                RasterizerStateDescription.Default,
                PrimitiveTopology.TriangleList,
                shaderSet,
                new[] { projViewLayout, worldTextureLayout },
                gd.SwapchainFramebuffer.OutputDescription));

            _projViewSet = Storage.GetResourceSet(factory, new ResourceSetDescription(
                projViewLayout,
                _projectionBuffer,
                _viewBuffer));

            _worldTextureSet = Storage.GetResourceSet(factory, new ResourceSetDescription(
                worldTextureLayout,
                _worldBuffer,
                _surfaceTextureView,
                gd.Aniso4xSampler));
        }

        public override void UpdatePerFrameResources(CommandList cl)
        {
            // while nothing
        }

        protected override void SetGraphicsSet(CommandList cl)
        {
            cl.SetGraphicsResourceSet(0, _projViewSet);
            cl.SetGraphicsResourceSet(1, _worldTextureSet);
        }
    }

    public struct VertexPositionTexture
    {
        public const uint SizeInBytes = 20;

        public float PosX;
        public float PosY;
        public float PosZ;

        public float TexU;
        public float TexV;

        public VertexPositionTexture(Vector3 pos, Vector2 uv)
        {
            PosX = pos.X;
            PosY = pos.Y;
            PosZ = pos.Z;
            TexU = uv.X;
            TexV = uv.Y;
        }
    }
}
