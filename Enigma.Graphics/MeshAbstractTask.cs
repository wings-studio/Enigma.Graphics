namespace Enigma.Graphics
{
    public abstract class MeshAbstractTask<T> : IRenderTask where T : unmanaged
    {
        public virtual void CreateResources(IGraphicsDevice gd, Scene scene)
        {
            if (scene is MeshScene<T> ms)
                CreateResources(gd, ms);
            else
                throw new System.NotSupportedException(
                    $"{GetType().FullName} is Mesh Rendering Task but {nameof(scene)} is not {nameof(MeshScene<T>)}");
        }

        public virtual void Render(IGraphicsDevice gd, Scene scene)
        {
            if (scene is MeshScene<T> ms)
                Render(gd, ms);
            else
                throw new System.NotSupportedException(
                    $"{GetType().FullName} is Mesh Rendering Task but {nameof(scene)} is not {nameof(MeshScene<T>)}");
        }

        public virtual void Render(IGraphicsDevice gd, MeshScene<T> scene)
        {
            for (int i = 0; i < scene.vertexBuffers.Count; i++)
                Render(gd, scene, i);
        }

        public abstract void CreateResources(IGraphicsDevice gd, MeshScene<T> scene);

        public abstract void Render(IGraphicsDevice gd, MeshScene<T> scene, int meshIndex);
    }
}
