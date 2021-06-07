namespace Enigma.Graphics.Objects
{
    public interface IMeshData<T> : Veldrid.Utilities.MeshData where T : unmanaged, IVertexInfo
    {
    }
}
