namespace Enigma.Graphics.Objects
{
    public interface IMeshData<T> : Veldrid.Utilities.MeshData where T : unmanaged, IVertexInfo
    {
        System.Collections.Generic.List<T> GetVertices(System.Numerics.Vector3[] vertices);
    }
}
