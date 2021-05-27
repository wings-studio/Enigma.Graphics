using System;
using Veldrid;
using Veldrid.ImageSharp;

namespace Enigma.Graphics
{
    public class RealtimeStorage : IGraphicsStorage
    {
        public unsafe Texture GetColorTexture(GraphicsDevice gd, ResourceFactory factory, RgbaByte color)
        {
            Texture _colorTex = factory.CreateTexture(TextureDescription.Texture2D(1, 1, 1, 1, PixelFormat.R8_G8_B8_A8_UNorm, TextureUsage.Sampled));
            gd.UpdateTexture(_colorTex, (IntPtr)(&color), Util.SizeOf<RgbaByte>(), 0, 0, 0, 1, 1, 1, 0, 0);
            return _colorTex;
        }

        public Pipeline GetPipeline(ResourceFactory factory, GraphicsPipelineDescription gpd)
        {
            return factory.CreateGraphicsPipeline(gpd);
        }

        public Pipeline GetPipeline(ResourceFactory resourceFactory, ref GraphicsPipelineDescription gpd)
        {
            return resourceFactory.CreateGraphicsPipeline(ref gpd);
        }

        public ResourceLayout GetResourceLayout(ResourceFactory resourceFactory, ResourceLayoutDescription resourceLayoutDescription)
        {
            return resourceFactory.CreateResourceLayout(resourceLayoutDescription);
        }

        public ResourceSet GetResourceSet(ResourceFactory sharedFactory, ResourceSetDescription resourceSetDescription)
        {
            return sharedFactory.CreateResourceSet(resourceSetDescription);
        }

        public (Shader vs, Shader fs) GetShaders(GraphicsDevice gd, ResourceFactory factory, string name, bool fromResources = false)
        {
            return Shaders.ShaderHelper.LoadSPIRV(gd, factory, name, fromResources);
        }

        public Texture GetTexture2D(GraphicsDevice gd, ResourceFactory factory, ImageSharpTexture textureData)
        {
            return textureData.CreateDeviceTexture(gd, factory);
        }

        public TextureView GetTextureView(ResourceFactory factory, Texture texture)
        {
            return factory.CreateTextureView(texture);
        }
    }
}
