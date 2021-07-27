namespace Enigma.Graphics
{
    public interface IBuffer : System.IDisposable, IResource
    {
        BufferUsage Usage { set; get; }
    }
}
