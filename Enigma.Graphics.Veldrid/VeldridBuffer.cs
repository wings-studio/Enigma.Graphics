using System;
using Veldrid;

namespace Enigma.Graphics.Veldrid
{
    public class VeldridBuffer : IBuffer
    {
        public readonly DeviceBuffer DeviceBuffer;
        public BufferUsage Usage 
        {
            get => VeldridUtil.FromVeldridBuffer(DeviceBuffer.Usage);
            set => throw new EnigmaVeldridException("You cannot change usage of buffer after creating it"); 
        }

        public VeldridBuffer(DeviceBuffer deviceBuffer)
        {
            DeviceBuffer = deviceBuffer;
        }

        public void Dispose()
        {
            DeviceBuffer.Dispose();
        }

        public void SetResources(IGraphicsDevice graphicsDevice)
        {
            graphicsDevice.SetUniformBuffer(this);
        }
    }
}
