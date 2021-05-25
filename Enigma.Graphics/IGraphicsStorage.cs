using System;
using System.Collections.Generic;
using Veldrid;
using Veldrid.ImageSharp;

namespace Enigma.Graphics
{
    public interface IGraphicsStorage
    {
        ResourceLayoutDescription ProjViewLayoutDescription => new ResourceLayoutDescription(
            new ResourceLayoutElementDescription("Projection", ResourceKind.UniformBuffer, ShaderStages.Vertex),
            new ResourceLayoutElementDescription("View", ResourceKind.UniformBuffer, ShaderStages.Vertex));

        Pipeline GetPipeline(ResourceFactory factory, GraphicsPipelineDescription gpd);
        (Shader vs, Shader fs) GetShaders(GraphicsDevice gd, ResourceFactory factory, string name);
        Texture GetTexture2D(GraphicsDevice gd, ResourceFactory resourceFactory, ImageSharpTexture textureData);
        ResourceSet GetResourceSet(ResourceFactory sharedFactory, ResourceSetDescription resourceSetDescription);
        ResourceLayout GetResourceLayout(ResourceFactory resourceFactory, ResourceLayoutDescription resourceLayoutDescription);
        TextureView GetTextureView(ResourceFactory resourceFactory, Texture alphamapTexture);
        Pipeline GetPipeline(ResourceFactory resourceFactory, ref GraphicsPipelineDescription depthPD);
    }
}
