using System;
using Silk.NET.OpenGL;

namespace Enigma.Graphics.Silk.OpenGL
{
    public class GlBuffer : IBuffer
    {
        public uint GlCode;
        public uint Size { get; }
        public virtual BufferUsage Usage { get; set; }

        protected readonly GL gl;

        public GlBuffer(GL gl)
        {
            this.gl = gl;
            GlCode = gl.GenBuffer();
        }
        public GlBuffer(GL gl, int size, BufferUsage usage) : this(gl)
        {
            Size = Convert.ToUInt32(size);
            Usage = usage;
        }

        public virtual void Bind()
        {
            gl.BindBuffer(GlUtil.FromEnigmaBuffer(Usage), GlCode);
        }

        public void Dispose()
        {
            gl.DeleteBuffer(GlCode);
        }

        public void SetResources(IGraphicsDevice graphicsDevice)
        {
            graphicsDevice.SetUniformBuffer(this);
        }
    }
}
