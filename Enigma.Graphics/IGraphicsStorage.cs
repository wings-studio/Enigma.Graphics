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
        (Shader vs, Shader fs) GetShaders(GraphicsDevice gd, ResourceFactory factory, string name, bool fromResources = false);
        Texture GetColorTexture(GraphicsDevice gd, ResourceFactory factory, RgbaByte color);
        Texture GetTexture2D(GraphicsDevice gd, ResourceFactory factory, ImageSharpTexture textureData);
        ResourceSet GetResourceSet(ResourceFactory factory, ResourceSetDescription resourceSetDescription);
        ResourceLayout GetResourceLayout(ResourceFactory factory, ResourceLayoutDescription resourceLayoutDescription);
        TextureView GetTextureView(ResourceFactory factory, Texture texture);
        Pipeline GetPipeline(ResourceFactory factory, ref GraphicsPipelineDescription gpd);
    }
}
