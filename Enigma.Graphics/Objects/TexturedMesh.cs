﻿using System;
using System.Numerics;
using Veldrid;
using Veldrid.ImageSharp;
using Veldrid.Utilities;

namespace Enigma.Graphics
{
    public class TexturedMesh : IRenderable
    {
        // Useful for testing uniform bindings with an offset.
        private static readonly bool s_useUniformOffset = false;
        private uint _uniformOffset = 0;

        private readonly string _name;
        private readonly MeshData _meshData;
        private readonly ImageSharpTexture _textureData;
        private readonly ImageSharpTexture _alphaTextureData;
        private readonly Transform _transform = new Transform();

        private BoundingBox _centeredBounds;
        private DeviceBuffer _vb;
        private DeviceBuffer _ib;
        private int _indexCount;
        private Texture _texture;
        private Texture _alphamapTexture;
        private TextureView _alphaMapView;

        private Pipeline _pipeline;
        private Pipeline _pipelineFrontCull;
        private ResourceSet _mainProjViewRS;
        private ResourceSet _mainSharedRS;
        private ResourceSet _mainPerObjectRS;
        private ResourceSet _reflectionRS;
        private ResourceSet _noReflectionRS;
        private Pipeline _shadowMapPipeline;
        private ResourceSet[] _shadowMapResourceSets;

        private DeviceBuffer _worldAndInverseBuffer;

        private readonly DisposeCollector _disposeCollector = new DisposeCollector();

        private readonly MaterialPropsAndBuffer _materialProps;
        private readonly Vector3 _objectCenter;
        private bool _materialPropsOwned = false;

        private IGraphicsStorage Storage => Renderer.Storage;

        public MaterialProperties MaterialProperties { get => _materialProps.Properties; set { _materialProps.Properties = value; } }

        public Transform Transform => _transform;

        public GraphicsDevice GraphicsDevice { get; set; }
        public CommandList CommandList { get; set; }

        public TexturedMesh(string name, MeshData meshData, ImageSharpTexture textureData, ImageSharpTexture alphaTexture, MaterialPropsAndBuffer materialProps)
        {
            _name = name;
            _meshData = meshData;
            _centeredBounds = meshData.GetBoundingBox();
            _objectCenter = _centeredBounds.GetCenter();
            _textureData = textureData;
            _alphaTextureData = alphaTexture;
            _materialProps = materialProps;
        }

        public override BoundingBox BoundingBox => BoundingBox.Transform(_centeredBounds, _transform.GetTransformMatrix());

        public override unsafe void CreateDeviceObjects(GraphicsDevice gd, CommandList cl)
        {
            if (s_useUniformOffset)
            {
                _uniformOffset = gd.UniformBufferMinOffsetAlignment;
            }
            ResourceFactory disposeFactory = new DisposeCollectorResourceFactory(gd.ResourceFactory, _disposeCollector);
            _vb = _meshData.CreateVertexBuffer(disposeFactory, cl);
            _vb.Name = _name + "_VB";
            _ib = _meshData.CreateIndexBuffer(disposeFactory, cl, out _indexCount);
            _ib.Name = _name + "_IB";

            uint bufferSize = 128;
            if (s_useUniformOffset) { bufferSize += _uniformOffset * 2; }

            _worldAndInverseBuffer = disposeFactory.CreateBuffer(new BufferDescription(bufferSize, BufferUsage.UniformBuffer | BufferUsage.Dynamic));
            if (_materialPropsOwned)
            {
                _materialProps.CreateDeviceObjects(gd, cl, sc);
            }

            if (_textureData != null)
            {
                _texture = Storage.GetTexture2D(gd, gd.ResourceFactory, _textureData);
            }
            else
            {
                _texture = gd.GetColorTexture(gd.ResourceFactory, RgbaByte.Pink);
            }

            if (_alphaTextureData != null)
            {
                _alphamapTexture = _alphaTextureData.CreateDeviceTexture(gd, disposeFactory);
            }
            else
            {
                _alphamapTexture = gd.GetColorTexture(gd.ResourceFactory, RgbaByte.Pink);
            }
            _alphaMapView = Storage.GetTextureView(gd.ResourceFactory, _alphamapTexture);

            VertexLayoutDescription[] shadowDepthVertexLayouts = new VertexLayoutDescription[]
            {
                new VertexLayoutDescription(
                    new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
                    new VertexElementDescription("Normal", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
                    new VertexElementDescription("TexCoord", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2))
            };

            (Shader depthVS, Shader depthFS) = Storage.GetShaders(gd, gd.ResourceFactory, "ShadowDepth");

            ResourceLayout projViewCombinedLayout = Storage.GetResourceLayout(
                gd.ResourceFactory,
                new ResourceLayoutDescription(
                    new ResourceLayoutElementDescription("ViewProjection", ResourceKind.UniformBuffer, ShaderStages.Vertex)));

            ResourceLayout worldLayout = Storage.GetResourceLayout(gd.ResourceFactory, new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("WorldAndInverse", ResourceKind.UniformBuffer, ShaderStages.Vertex, ResourceLayoutElementOptions.DynamicBinding)));

            GraphicsPipelineDescription depthPD = new GraphicsPipelineDescription(
                BlendStateDescription.Empty,
                gd.IsDepthRangeZeroToOne ? DepthStencilStateDescription.DepthOnlyGreaterEqual : DepthStencilStateDescription.DepthOnlyLessEqual,
                RasterizerStateDescription.Default,
                PrimitiveTopology.TriangleList,
                new ShaderSetDescription(
                    shadowDepthVertexLayouts,
                    new[] { depthVS, depthFS },
                    new[] { new SpecializationConstant(100, gd.IsClipSpaceYInverted) }),
                new ResourceLayout[] { projViewCombinedLayout, worldLayout },
                sc.NearShadowMapFramebuffer.OutputDescription);
            _shadowMapPipeline = Storage.GetPipeline(gd.ResourceFactory, ref depthPD);

            _shadowMapResourceSets = CreateShadowMapResourceSets(gd.ResourceFactory, disposeFactory, sc, projViewCombinedLayout, worldLayout);

            VertexLayoutDescription[] mainVertexLayouts = new VertexLayoutDescription[]
            {
                new VertexLayoutDescription(
                    new VertexElementDescription("Position", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
                    new VertexElementDescription("Normal", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float3),
                    new VertexElementDescription("TexCoord", VertexElementSemantic.TextureCoordinate, VertexElementFormat.Float2))
            };

            (Shader mainVS, Shader mainFS) = Storage.GetShaders(gd, gd.ResourceFactory, "ShadowMain");

            ResourceLayout projViewLayout = Storage.GetResourceLayout(
                gd.ResourceFactory,
                Storage.ProjViewLayoutDescription);

            ResourceLayout mainSharedLayout = Storage.GetResourceLayout(gd.ResourceFactory, new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("LightViewProjection1", ResourceKind.UniformBuffer, ShaderStages.Vertex | ShaderStages.Fragment),
                new ResourceLayoutElementDescription("LightViewProjection2", ResourceKind.UniformBuffer, ShaderStages.Vertex | ShaderStages.Fragment),
                new ResourceLayoutElementDescription("LightViewProjection3", ResourceKind.UniformBuffer, ShaderStages.Vertex | ShaderStages.Fragment),
                new ResourceLayoutElementDescription("DepthLimits", ResourceKind.UniformBuffer, ShaderStages.Vertex | ShaderStages.Fragment),
                new ResourceLayoutElementDescription("LightInfo", ResourceKind.UniformBuffer, ShaderStages.Vertex | ShaderStages.Fragment),
                new ResourceLayoutElementDescription("CameraInfo", ResourceKind.UniformBuffer, ShaderStages.Vertex | ShaderStages.Fragment),
                new ResourceLayoutElementDescription("PointLights", ResourceKind.UniformBuffer, ShaderStages.Vertex | ShaderStages.Fragment)));

            ResourceLayout mainPerObjectLayout = Storage.GetResourceLayout(gd.ResourceFactory, new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("WorldAndInverse", ResourceKind.UniformBuffer, ShaderStages.Vertex | ShaderStages.Fragment, ResourceLayoutElementOptions.DynamicBinding),
                new ResourceLayoutElementDescription("MaterialProperties", ResourceKind.UniformBuffer, ShaderStages.Vertex | ShaderStages.Fragment),
                new ResourceLayoutElementDescription("SurfaceTexture", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                new ResourceLayoutElementDescription("RegularSampler", ResourceKind.Sampler, ShaderStages.Fragment),
                new ResourceLayoutElementDescription("AlphaMap", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                new ResourceLayoutElementDescription("AlphaMapSampler", ResourceKind.Sampler, ShaderStages.Fragment),
                new ResourceLayoutElementDescription("ShadowMapNear", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                new ResourceLayoutElementDescription("ShadowMapMid", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                new ResourceLayoutElementDescription("ShadowMapFar", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                new ResourceLayoutElementDescription("ShadowMapSampler", ResourceKind.Sampler, ShaderStages.Fragment)));

            ResourceLayout reflectionLayout = Storage.GetResourceLayout(gd.ResourceFactory, new ResourceLayoutDescription(
                new ResourceLayoutElementDescription("ReflectionMap", ResourceKind.TextureReadOnly, ShaderStages.Fragment),
                new ResourceLayoutElementDescription("ReflectionSampler", ResourceKind.Sampler, ShaderStages.Fragment),
                new ResourceLayoutElementDescription("ReflectionViewProj", ResourceKind.UniformBuffer, ShaderStages.Vertex),
                new ResourceLayoutElementDescription("ClipPlaneInfo", ResourceKind.UniformBuffer, ShaderStages.Fragment)));

            BlendStateDescription alphaBlendDesc = BlendStateDescription.SingleAlphaBlend;
            alphaBlendDesc.AlphaToCoverageEnabled = true;

            GraphicsPipelineDescription mainPD = new GraphicsPipelineDescription(
                _alphamapTexture != null ? alphaBlendDesc : BlendStateDescription.SingleOverrideBlend,
                gd.IsDepthRangeZeroToOne ? DepthStencilStateDescription.DepthOnlyGreaterEqual : DepthStencilStateDescription.DepthOnlyLessEqual,
                RasterizerStateDescription.Default,
                PrimitiveTopology.TriangleList,
                new ShaderSetDescription(mainVertexLayouts, new[] { mainVS, mainFS }, new[] { new SpecializationConstant(100, gd.IsClipSpaceYInverted) }),
                new ResourceLayout[] { projViewLayout, mainSharedLayout, mainPerObjectLayout, reflectionLayout },
                sc.MainSceneFramebuffer.OutputDescription);
            _pipeline = Storage.GetPipeline(gd.ResourceFactory, ref mainPD);
            _pipeline.Name = "TexturedMesh Main Pipeline";
            mainPD.RasterizerState.CullMode = FaceCullMode.Front;
            mainPD.Outputs = sc.ReflectionFramebuffer.OutputDescription;
            _pipelineFrontCull = Storage.GetPipeline(gd.ResourceFactory, ref mainPD);

            _mainProjViewRS = Storage.GetResourceSet(gd.ResourceFactory, new ResourceSetDescription(projViewLayout,
                sc.ProjectionMatrixBuffer,
                sc.ViewMatrixBuffer));

            _mainSharedRS = Storage.GetResourceSet(gd.ResourceFactory, new ResourceSetDescription(mainSharedLayout,
                sc.LightViewProjectionBuffer0,
                sc.LightViewProjectionBuffer1,
                sc.LightViewProjectionBuffer2,
                sc.DepthLimitsBuffer,
                sc.LightInfoBuffer,
                sc.CameraInfoBuffer,
                sc.PointLightsBuffer));

            _mainPerObjectRS = disposeFactory.CreateResourceSet(new ResourceSetDescription(mainPerObjectLayout,
                new DeviceBufferRange(_worldAndInverseBuffer, _uniformOffset, 128),
                _materialProps.UniformBuffer,
                _texture,
                gd.Aniso4xSampler,
                _alphaMapView,
                gd.LinearSampler,
                sc.NearShadowMapView,
                sc.MidShadowMapView,
                sc.FarShadowMapView,
                gd.PointSampler));

            _reflectionRS = Storage.GetResourceSet(gd.ResourceFactory, new ResourceSetDescription(reflectionLayout,
                _alphaMapView, // Doesn't really matter -- just don't bind the actual reflection map since it's being rendered to.
                gd.PointSampler,
                sc.ReflectionViewProjBuffer,
                sc.MirrorClipPlaneBuffer));

            _noReflectionRS = Storage.GetResourceSet(gd.ResourceFactory, new ResourceSetDescription(reflectionLayout,
                sc.ReflectionColorView,
                gd.PointSampler,
                sc.ReflectionViewProjBuffer,
                sc.NoClipPlaneBuffer));
        }

        private ResourceSet[] CreateShadowMapResourceSets(
            ResourceFactory sharedFactory,
            ResourceFactory disposeFactory,
            ResourceLayout projViewLayout,
            ResourceLayout worldLayout)
        {
            ResourceSet[] ret = new ResourceSet[6];

            for (int i = 0; i < 3; i++)
            {
                DeviceBuffer viewProjBuffer = i == 0 ? sc.LightViewProjectionBuffer0 : i == 1 ? sc.LightViewProjectionBuffer1 : sc.LightViewProjectionBuffer2;
                ret[i * 2] = Storage.GetResourceSet(sharedFactory, new ResourceSetDescription(
                    projViewLayout,
                    viewProjBuffer));
                ResourceSet worldRS = disposeFactory.CreateResourceSet(new ResourceSetDescription(
                    worldLayout,
                    new DeviceBufferRange(_worldAndInverseBuffer, _uniformOffset, 128)));
                ret[i * 2 + 1] = worldRS;
            }

            return ret;
        }

        public override void Dispose()
        {
            if (_materialPropsOwned)
            {
                _materialProps.DestroyDeviceObjects();
            }

            _disposeCollector.DisposeAll();
        }              

        public void Render()
        {
            if (_materialPropsOwned)
            {
                _materialProps.FlushChanges(CommandList);
            }

            if ((renderPass & RenderPasses.AllShadowMap) != 0)
            {
                int shadowMapIndex = renderPass == RenderPasses.ShadowMapNear ? 0 : renderPass == RenderPasses.ShadowMapMid ? 1 : 2;
                RenderShadowMap(CommandList, shadowMapIndex);
            }
            else if (renderPass == RenderPasses.Standard || renderPass == RenderPasses.AlphaBlend)
            {
                RenderStandard(CommandList, false);
            }
            else if (renderPass == RenderPasses.ReflectionMap)
            {
                RenderStandard(CommandList, true);
            }
        }

        public void UpdatePerFrameResources()
        {
            WorldAndInverse wai;
            wai.World = _transform.GetTransformMatrix();
            wai.InverseWorld = VdUtilities.CalculateInverseTranspose(ref wai.World);
            GraphicsDevice.UpdateBuffer(_worldAndInverseBuffer, _uniformOffset * 2, ref wai);
        }

        private void RenderShadowMap(CommandList cl, int shadowMapIndex)
        {
            cl.SetVertexBuffer(0, _vb);
            cl.SetIndexBuffer(_ib, IndexFormat.UInt16);
            cl.SetPipeline(_shadowMapPipeline);
            cl.SetGraphicsResourceSet(0, _shadowMapResourceSets[shadowMapIndex * 2]);
            uint offset = _uniformOffset;
            cl.SetGraphicsResourceSet(1, _shadowMapResourceSets[shadowMapIndex * 2 + 1], 1, ref offset);
            cl.DrawIndexed((uint)_indexCount, 1, 0, 0, 0);
        }

        private void RenderStandard(CommandList cl, bool reflectionPass)
        {
            cl.SetVertexBuffer(0, _vb);
            cl.SetIndexBuffer(_ib, IndexFormat.UInt16);
            cl.SetPipeline(reflectionPass ? _pipelineFrontCull : _pipeline);
            cl.SetGraphicsResourceSet(0, _mainProjViewRS);
            cl.SetGraphicsResourceSet(1, _mainSharedRS);
            uint offset = _uniformOffset;
            cl.SetGraphicsResourceSet(2, _mainPerObjectRS, 1, ref offset);
            cl.SetGraphicsResourceSet(3, reflectionPass ? _reflectionRS : _noReflectionRS);
            cl.DrawIndexed((uint)_indexCount, 1, 0, 0, 0);
        }
    }

    public struct WorldAndInverse
    {
        public Matrix4x4 World;
        public Matrix4x4 InverseWorld;
    }
}