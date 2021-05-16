using System;
using System.Collections.Generic;
using System.Numerics;
using Assimp;
using Veldrid;
using Veldrid.Utilities;

namespace Enigma.Graphics.Objects
{
    public class AssimpMesh : MeshData
    {
        public Vector3 Center { private set; get; }

        private readonly Mesh mesh;

        public AssimpMesh(Mesh mesh)
        {
            this.mesh = mesh;
            Center = GetBoundingBox().GetCenter();
        }

        public DeviceBuffer CreateIndexBuffer(ResourceFactory factory, CommandList cl, out int indexCount)
        {
            ushort[] indicies = GetIndices();
            indexCount = indicies.Length;
            BufferDescription desc = new BufferDescription(indicies.SizeInBytes(), BufferUsage.IndexBuffer);
            DeviceBuffer buffer = factory.CreateBuffer(desc);
            cl.UpdateBuffer(buffer, 0, indicies);
            return buffer;
        }

        public DeviceBuffer CreateVertexBuffer(ResourceFactory factory, CommandList cl)
        {
            DeviceBuffer buffer;
            if (mesh.HasVertices)
            {
                var vertices = mesh.Vertices.ConvertToVectorArray();
                BufferDescription desc = new BufferDescription() { Usage = BufferUsage.VertexBuffer };
                buffer = factory.CreateBuffer(desc);
                cl.UpdateBuffer(buffer, 0, vertices);
            }
            else
            {
                buffer = factory.CreateBuffer(new BufferDescription() { Usage = BufferUsage.VertexBuffer });
            }
            return buffer;
        }

        public Veldrid.Utilities.BoundingBox GetBoundingBox()
        {
            var bb = mesh.BoundingBox;
            return new Veldrid.Utilities.BoundingBox(bb.Min.ToNumerics(), bb.Max.ToNumerics());
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

        public bool RayCast(Veldrid.Utilities.Ray ray, out float distance)
        {
            if (ray.Intersects(GetBoundingBox()))
            {
                distance = Vector3.Distance(ray.Origin, Center);
                return true;
            }
            distance = 0;
            return false;
        }

        public int RayCast(Veldrid.Utilities.Ray ray, List<float> distances)
        {
            return 0;
        }
    }
}
