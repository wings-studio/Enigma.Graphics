using System;
using System.Collections.Generic;
using System.Numerics;
using Veldrid;

namespace Enigma.Graphics.Objects
{
    public class TexturedMesh : Mesh<VertexPositionTexture>
    {
        public string TexturePath { get; set; }

        private ResourceSet _projViewSet;
        private ResourceSet _worldTextureSet;
        private DeviceBuffer _projectionBuffer;
        private DeviceBuffer _viewBuffer;
        private DeviceBuffer _worldBuffer;

        private const string VertexCode = @"
#version 450
layout(set = 0, binding = 0) uniform ProjectionBuffer
{
    mat4 Projection;
};
layout(set = 0, binding = 1) uniform ViewBuffer
{
    mat4 View;
};
layout(set = 1, binding = 0) uniform WorldBuffer
{
    mat4 World;
};
layout(location = 0) in vec3 Position;
layout(location = 1) in vec2 TexCoords;
layout(location = 0) out vec2 fsin_texCoords;
void main()
{
    vec4 worldPosition = World * vec4(Position, 1);
    vec4 viewPosition = View * worldPosition;
    vec4 clipPosition = Projection * viewPosition;
    gl_Position = clipPosition;
    fsin_texCoords = TexCoords;
}";

        private const string FragmentCode = @"
#version 450
layout(location = 0) in vec2 fsin_texCoords;
layout(location = 0) out vec4 fsout_color;
layout(set = 1, binding = 1) uniform texture2D SurfaceTexture;
layout(set = 1, binding = 2) uniform sampler SurfaceSampler;
void main()
{
    fsout_color =  texture(sampler2D(SurfaceTexture, SurfaceSampler), fsin_texCoords);
}";

        public TexturedMesh(IMeshData<VertexPositionTexture> data) : base(data) { }

        public override void CreateDeviceObjects(GraphicsDevice gd, CommandList cl)
        {
            base.CreateDeviceObjects(gd, cl);

            _projectionBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));
            _viewBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));
            _worldBuffer = factory.CreateBuffer(new BufferDescription(64, BufferUsage.UniformBuffer));

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
            (Shader vs, Shader fs) =
                Shaders.ShaderHelper.LoadSPIRV(gd, factory, VertexCode, FragmentCode);
                //Storage.GetShaders(gd, factory, "BaseMesh");

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

        protected override void SetGraphicsSet(CommandList cl, Camera camera)
        {
            //cl.UpdateBuffer(_projectionBuffer, 0, camera.ProjectionMatrix);
            //cl.UpdateBuffer(_viewBuffer, 0, camera.ViewMatrix);
            cl.UpdateBuffer(_projectionBuffer, 0, Matrix4x4.CreatePerspectiveFieldOfView(
                1.0f,
                camera.AspectRatio,
                0.5f,
                100f));
            cl.UpdateBuffer(_viewBuffer, 0, camera.ViewMatrix); //Matrix4x4.CreateLookAt(Vector3.UnitZ * 2.5f, Vector3.Zero, Vector3.UnitY));
            cl.UpdateBuffer(_worldBuffer, 0, Transform.GetTransformMatrix());

            cl.SetGraphicsResourceSet(0, _projViewSet);
            cl.SetGraphicsResourceSet(1, _worldTextureSet);
        }
    }

    public struct VertexPositionTexture : IVertexInfo
    {
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

        public void SetVertex(Vector3 vertexCoord)
        {
            PosX = vertexCoord.X;
            PosY = vertexCoord.Y;
            PosZ = vertexCoord.Z;
            TexU = 0;
            TexV = 0;
        }

        public uint SizeInBytes() => 20;
    }
}
