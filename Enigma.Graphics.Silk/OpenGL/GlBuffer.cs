using System;
using Silk.NET.OpenGL;

namespace Enigma.Graphics.Silk.OpenGL
{
    public class GlBuffer : IBuffer
    {
        public uint GlCode;
        public uint Size => size;
        public IntPtr Data
        {
            get
            {
                gl.GetBufferSubData(GlUtil.FromEnigmaBuffer(Usage), 0, Size, out IntPtr data);
                return data;
            }
        }
        public virtual BufferUsage Usage { get; set; }

        protected readonly GL gl;
        protected readonly uint size;

        public GlBuffer(GL gl)
        {
            this.gl = gl;
            GlCode = gl.GenBuffer();
        }
        public GlBuffer(GL gl, int size, BufferUsage usage) : this(gl)
        {
            this.size = Convert.ToUInt32(size);
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
    }
}
