using System;
using System.Collections.Generic;
using System.Numerics;

namespace Enigma.Graphics
{
    public class MeshScene<T> : Scene where T : unmanaged
    {
        public readonly List<Mesh<T>> meshes = new List<Mesh<T>>();
        public readonly List<IBuffer> indexBuffers = new List<IBuffer>(), vertexBuffers = new List<IBuffer>();
        public readonly List<uint> indexLens = new List<uint>();

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
            foreach (var mesh in meshes)
            {
                indexBuffers.Add(mesh.CreateIndexBuffer(graphicsDevice));
                indexLens.Add((uint)mesh.Indicies.Length);
                vertexBuffers.Add(mesh.CreateVertexBuffer(graphicsDevice));
            }
            base.CreateResources(graphicsDevice);
        }

        public override void Render(IGraphicsDevice gd)
        {
            for (int i = 0; i < meshes.Count; i++)
            {
                foreach (MeshAbstractTask<T> meshTask in meshTasks)
                    meshTask.Render(gd, this, i);
            }
            base.Render(gd);
        }
    }
}
