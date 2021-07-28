using System;
using System.Runtime.InteropServices;

namespace Enigma.Graphics
{
    public class Resource<T> : IResource where T : unmanaged
    {
        public T Value;

        public Resource(T value)
        {
            Value = value;
        }

        public unsafe IBuffer GetBuffer(IGraphicsDevice graphicsDevice, BufferUsage bufferUsage)
        {
            IBuffer buf = graphicsDevice.CreateBuffer(sizeof(T), bufferUsage);
            graphicsDevice.UpdateBuffer(buf, Value);
            return buf;
        }
    }
}
