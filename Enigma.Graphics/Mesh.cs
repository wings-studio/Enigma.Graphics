using System;
using System.Collections.Generic;
using System.Numerics;

namespace Enigma.Graphics
{
    public class Mesh<T> where T : unmanaged
    {
        public Transform Transform;
        public T[] Vertices;
        public uint[] Indicies;

        public Mesh()
        {
            Vertices = new T[0];
            Indicies = new uint[0];
        }
        public Mesh(T[] vertices, uint[] indicies)
        {
            Vertices = vertices;
            Indicies = indicies;
        }

        public IBuffer CreateVertexBuffer(IGraphicsDevice gd)
        {
            IBuffer vertexBuffer = gd.CreateBuffer(Util.Sizeof(Vertices), BufferUsage.VertexBuffer);
            gd.UpdateBuffer(vertexBuffer, Vertices);
            return vertexBuffer;
        }

        public IBuffer CreateIndexBuffer(IGraphicsDevice gd)
        {
            IBuffer indexBuffer = gd.CreateBuffer(Util.Sizeof(Indicies), BufferUsage.IndexBuffer);
            gd.UpdateBuffer(indexBuffer, Indicies);
            return indexBuffer;
        }
    }
}
