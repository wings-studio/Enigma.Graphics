using System.Runtime.InteropServices;
using Silk.NET.OpenGL;

namespace Enigma.Graphics.Silk.OpenGL
{
    public class GlVertexArray : GlBuffer
    {
        public uint GlArrayCode;
        public override BufferUsage Usage { get => BufferUsage.VertexBuffer; set { } }

        public unsafe GlVertexArray(GL gl, int size) : base(gl, size, BufferUsage.VertexBuffer)
        {
            GlArrayCode = gl.GenVertexArray();
        }

        public override void Bind()
        {
            gl.BindVertexArray(GlArrayCode);
            base.Bind();
        }
    }
}
