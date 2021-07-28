namespace Enigma.Graphics
{
    public interface IResource
    {
        IBuffer GetBuffer(IGraphicsDevice graphicsDevice, BufferUsage bufferUsage);
    }
}
