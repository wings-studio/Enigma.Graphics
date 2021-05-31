using System;
using System.Collections.Generic;
using System.Numerics;
using Veldrid;
using Veldrid.Utilities;
using AMesh = Assimp.Mesh;

namespace Enigma.Graphics.Objects
{
    public class AssimpMesh<T> : IMeshData<T> where T : unmanaged, IVertexInfo
    {
        public Vector3 Center { private set; get; }

        public static uint VertexSize = default(T).SizeInBytes();

        private readonly AMesh mesh;

        public AssimpMesh(AMesh mesh)
        {
            this.mesh = mesh;
            Center = GetBoundingBox().GetCenter();
        }

        public DeviceBuffer CreateIndexBuffer(ResourceFactory factory, CommandList cl, out int indexCount)
        {
            ushort[] indicies = GetIndices();
            indexCount = indicies.Length;
            BufferDescription desc = new BufferDescription((uint)(indicies.Length * sizeof(uint)), BufferUsage.IndexBuffer);
            DeviceBuffer buffer = factory.CreateBuffer(desc);
            cl.UpdateBuffer(buffer, 0, indicies);
            return buffer;
        }

        public DeviceBuffer CreateVertexBuffer(ResourceFactory factory, CommandList cl)
        {
            DeviceBuffer buffer;
            if (mesh.HasVertices)
            {
                BufferDescription desc = new ((uint)(mesh.VertexCount * VertexSize), BufferUsage.VertexBuffer);
                buffer = factory.CreateBuffer(desc);
                cl.UpdateBuffer(buffer, 0, GetVertices(GetVertexPositions()).ToArray());
            }
            else
            {
                buffer = factory.CreateBuffer(new BufferDescription() { Usage = BufferUsage.VertexBuffer });
            }
            return buffer;
        }

        public BoundingBox GetBoundingBox()
        {
            var bb = mesh.BoundingBox;
            return new BoundingBox(bb.Min.ToNumerics(), bb.Max.ToNumerics());
        }

        public BoundingSphere GetBoundingSphere()
        {
            return new BoundingSphere(Center, Vector3.Distance(Center, mesh.BoundingBox.Max.ToNumerics()));
        }

        public ushort[] GetIndices()
        {
            uint[] _indicies = mesh.GetUnsignedIndices();
            ushort[] indicies = new ushort[_indicies.Length];
            for (int i = 0; i < _indicies.Length; i++)
            {
                indicies[i] = Convert.ToUInt16(_indicies[i]);
            }
            return indicies;
        }

        public Vector3[] GetVertexPositions() => mesh.Vertices.ConvertToVectorArray();

        public bool RayCast(Ray ray, out float distance)
        {
            if (ray.Intersects(GetBoundingBox()))
            {
                distance = Vector3.Distance(ray.Origin, Center);
                return true;
            }
            distance = 0;
            return false;
        }

        public int RayCast(Ray ray, List<float> distances)
        {
            return 0;
        }

        public List<T> GetVertices(Vector3[] vertices)
        {
            List<T> _vertices = new List<T>();
            foreach (Vector3 vertex in vertices)
            {
                T v = new T();
                v.SetVertex(vertex);
                _vertices.Add(v);
            }
            return _vertices;
        }
    }
}
