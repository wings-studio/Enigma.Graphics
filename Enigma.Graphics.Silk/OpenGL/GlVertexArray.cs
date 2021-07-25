using System.Runtime.InteropServices;
using Silk.NET.OpenGL;

namespace Enigma.Graphics.Silk.OpenGL
{
    public class GlVertexArray : GlBuffer
    {
        public uint GlArrayCode;
        public override BufferUsage Usage { get => BufferUsage.VertexBuffer; set { } }

        public unsafe GlVertexArray(GL gl, int size) : base(gl)
        {
            GlArrayCode = gl.GenVertexArray();
            gl.BufferData(BufferTargetARB.ArrayBuffer, (nuint)size, Marshal.AllocHGlobal(size).ToPointer(), BufferUsageARB.StaticDraw);
        }

        public override void Bind()
        {
            gl.BindVertexArray(GlArrayCode);
            base.Bind();
        }
    }
}
