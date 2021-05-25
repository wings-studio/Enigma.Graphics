using System;
using System.Collections.Generic;
using System.Linq;
using Veldrid;

namespace Enigma.Graphics.Objects
{
    public class BaseMesh : Mesh
    {
        public BaseMesh(Veldrid.Utilities.MeshData data) : base(data) { }

        public override void CreateDeviceObjects()
        {
            base.CreateDeviceObjects();

            VertexLayoutDescription[] mainVertexLayouts = new VertexLayoutDescription[]
            {
                new VertexLayoutDescription(
                    new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
                    new VertexElementDescription("Normal", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
                    new VertexElementDescription("TexCoord", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2))
            };
            ResourceLayout projViewLayout = Storage.GetResourceLayout(factory, Storage.ProjViewLayoutDescription);
            (Shader vs, Shader fs) = Storage.GetShaders(GraphicsDevice, factory, "BaseMesh");
            GraphicsPipelineDescription desc = new GraphicsPipelineDescription(
                BlendStateDescription.SingleOverrideBlend,
                GraphicsDevice.IsDepthRangeZeroToOne ? DepthStencilStateDescription.DepthOnlyGreaterEqual : DepthStencilStateDescription.DepthOnlyLessEqual,
                RasterizerStateDescription.Default,
                PrimitiveTopology.TriangleList,
                new ShaderSetDescription(mainVertexLayouts, new[] { vs, fs }, new[] { new SpecializationConstant(100, GraphicsDevice.IsClipSpaceYInverted) }),
                new ResourceLayout[] { projViewLayout },
                GraphicsDevice.SwapchainFramebuffer.OutputDescription);
            pipeline = Storage.GetPipeline(factory, desc);
        }

        public override void Render()
        {
            CommandList.SetVertexBuffer(0, vertexBuffer);
            CommandList.SetIndexBuffer(indexBuffer, IndexFormat.UInt16);
            CommandList.SetPipeline(pipeline);
            CommandList.DrawIndexed((uint)indexCount, 1, 0, 0, 0);
        }

        public override void UpdatePerFrameResources()
        {
            // while nothing
        }
    }
}
