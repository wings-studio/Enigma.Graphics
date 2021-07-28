using System;
using System.Collections.Generic;
using System.Numerics;

namespace Enigma.Graphics
{
    public class MeshScene<T> : Scene where T : unmanaged
    {
        public readonly List<Mesh<T>> Meshes = new List<Mesh<T>>();
        public readonly List<IBuffer> IndexBuffers = new List<IBuffer>(), VertexBuffers = new List<IBuffer>();
        public readonly List<uint> IndexLengths = new List<uint>();

        protected readonly List<MeshAbstractTask<T>> meshTasks = new List<MeshAbstractTask<T>>();

        public MeshScene()
        {
            AddRenderTask<MeshRenderTask<T>>();
        }

        public override void AddRenderTask<R>(R renderTask)
        {
            if (renderTask is MeshAbstractTask<T> meshTask)
                meshTasks.Add(meshTask);
            else
                base.AddRenderTask(renderTask);
        }

        public override void CreateResources(IGraphicsDevice graphicsDevice)
        {
            foreach (var mesh in Meshes)
            {
                IndexBuffers.Add(mesh.CreateIndexBuffer(graphicsDevice));
                IndexLengths.Add((uint)mesh.Indicies.Length);
                VertexBuffers.Add(mesh.CreateVertexBuffer(graphicsDevice));
            }
            foreach (MeshAbstractTask<T> meshTask in meshTasks)
                meshTask.CreateResources(graphicsDevice, this);
            base.CreateResources(graphicsDevice);
        }

        public override void Render(IGraphicsDevice gd)
        {
            for (int i = 0; i < Meshes.Count; i++)
            {
                foreach (MeshAbstractTask<T> meshTask in meshTasks)
                    meshTask.Render(gd, this, i);
            }
            base.Render(gd);
        }
    }
}
