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
        public ResourceLayoutDescription ProjViewLayoutDescription { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

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
            throw new NotImplementedException();
        }

        public ResourceSet GetResourceSet(ResourceFactory sharedFactory, ResourceSetDescription resourceSetDescription)
        {
            throw new NotImplementedException();
        }

        public (Shader vs, Shader fs) GetShaders(GraphicsDevice gd, ResourceFactory factory, string name)
        {
            return Shaders.ShaderHelper.LoadSPIRV(gd, factory, name);
        }

        public Texture GetTexture2D(GraphicsDevice gd, ResourceFactory resourceFactory, ImageSharpTexture textureData)
        {
            throw new NotImplementedException();
        }

        public TextureView GetTextureView(ResourceFactory resourceFactory, Texture alphamapTexture)
        {
            throw new NotImplementedException();
        }
    }
}
