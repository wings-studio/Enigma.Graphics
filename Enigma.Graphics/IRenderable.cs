using System;
using System.Numerics;
using Veldrid;
using Veldrid.Utilities;

namespace Enigma.Graphics
{
    public interface IRenderable : IDisposable
    {
        void UpdatePerFrameResources(GraphicsDevice gd, CommandList cl, SceneContext sc);
        void Render(GraphicsDevice gd, CommandList cl, SceneContext sc, RenderPasses renderPass);
        void CreateDeviceObjects(GraphicsDevice gd, CommandList cl, SceneContext sc);
        RenderOrderKey GetRenderOrderKey(Vector3 cameraPosition);
        RenderPasses RenderPasses => RenderPasses.Standard;
    }

    public abstract class CullRenderable : IRenderable
    {
        public bool Cull(ref BoundingFrustum visibleFrustum)
        {
            return visibleFrustum.Contains(BoundingBox) == ContainmentType.Disjoint;
        }

        public abstract void UpdatePerFrameResources(GraphicsDevice gd, CommandList cl, SceneContext sc);
        public abstract void Render(GraphicsDevice gd, CommandList cl, SceneContext sc, RenderPasses renderPass);
        public abstract void CreateDeviceObjects(GraphicsDevice gd, CommandList cl, SceneContext sc);
        public abstract RenderOrderKey GetRenderOrderKey(Vector3 cameraPosition);
        public abstract void Dispose();

        public abstract BoundingBox BoundingBox { get; }

        public abstract RenderPasses RenderPasses { get; }
    }
}
