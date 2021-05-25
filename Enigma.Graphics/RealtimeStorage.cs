using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Veldrid;
using Veldrid.ImageSharp;

namespace Enigma.Graphics
{
    public class RealtimeStorage : IGraphicsStorage
    {
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

        public (Shader vs, Shader fs) GetShaders(GraphicsDevice gd, ResourceFactory factory, string name)
        {
            return Shaders.ShaderHelper.LoadSPIRV(gd, factory, name);
        }

        public Texture GetTexture2D(GraphicsDevice gd, ResourceFactory factory, ImageSharpTexture textureData)
        {
            return textureData.CreateDeviceTexture(gd, factory);
        }

        public TextureView GetTextureView(ResourceFactory resourceFactory, Texture alphamapTexture)
        {
            throw new NotImplementedException();
        }
    }
}
