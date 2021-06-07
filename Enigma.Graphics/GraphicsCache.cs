using System;
using System.Collections.Generic;
using Veldrid;
using Veldrid.ImageSharp;

namespace Enigma.Graphics
{
    public class GraphicsCache : IGraphicsStorage
    {
        private readonly Dictionary<RgbaByte, Texture> colorTextures = new Dictionary<RgbaByte, Texture>();
        private readonly Dictionary<GraphicsPipelineDescription, Pipeline> pipelines = new Dictionary<GraphicsPipelineDescription, Pipeline>();
        private readonly Dictionary<ResourceLayoutDescription, ResourceLayout> resourceLayouts = new Dictionary<ResourceLayoutDescription, ResourceLayout>();

        private readonly RealtimeStorage realtimeStorage = new RealtimeStorage();

        public Texture GetColorTexture(GraphicsDevice gd, ResourceFactory factory, RgbaByte color)
        {
            if (colorTextures.TryGetValue(color, out Texture tex))
                return tex;
            else
            {
                Texture t = realtimeStorage.GetColorTexture(gd, factory, color);
                colorTextures.Add(color, t);
                return t;
            }
        }

        public Pipeline GetPipeline(ResourceFactory factory, GraphicsPipelineDescription gpd)
        {
            if (pipelines.TryGetValue(gpd, out Pipeline p))
                return p;
            else
            {
                Pipeline pip = realtimeStorage.GetPipeline(factory, gpd);
                pipelines.Add(gpd, pip);
                return pip;
            }
        }

        public Pipeline GetPipeline(ResourceFactory factory, ref GraphicsPipelineDescription gpd)
        {
            if (pipelines.TryGetValue(gpd, out Pipeline p))
                return p;
            else
            {
                Pipeline pip = realtimeStorage.GetPipeline(factory, ref gpd);
                pipelines.Add(gpd, pip);
                return pip;
            }
        }

        public ResourceLayout GetResourceLayout(ResourceFactory factory, ResourceLayoutDescription resourceLayoutDescription)
        {
            if (resourceLayouts.TryGetValue(resourceLayoutDescription, out ResourceLayout resout))
                return resout;
            else
            {
                ResourceLayout layout = realtimeStorage.GetResourceLayout(factory, resourceLayoutDescription);
                resourceLayouts.Add(resourceLayoutDescription, layout);
                return layout;
            }
        }

        public ResourceSet GetResourceSet(ResourceFactory factory, ResourceSetDescription resourceSetDescription)
        {
            throw new NotImplementedException();
        }

        public (Shader vs, Shader fs) GetShaders(GraphicsDevice gd, ResourceFactory factory, string name, bool fromResources = false)
        {
            throw new NotImplementedException();
        }

        public Texture GetTexture2D(GraphicsDevice gd, ResourceFactory factory, ImageSharpTexture textureData)
        {
            throw new NotImplementedException();
        }

        public TextureView GetTextureView(ResourceFactory factory, Texture texture)
        {
            throw new NotImplementedException();
        }
    }
}
