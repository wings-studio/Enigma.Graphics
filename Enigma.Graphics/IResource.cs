namespace Enigma.Graphics
{
    public interface IResource
    { 
        uint Size { get; }
        System.IntPtr Data { get; }
    }
}
