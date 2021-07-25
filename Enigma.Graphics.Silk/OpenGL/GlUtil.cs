using System;
using Silk.NET.OpenGL;
using System.Runtime.InteropServices;

namespace Enigma.Graphics.Silk.OpenGL
{
    public static class GlUtil
    {
        public static ShaderType FromEnigmaShader(ShaderStage shaderStage)
        {
            return shaderStage switch
            {
                ShaderStage.Vertex => ShaderType.VertexShader,
                ShaderStage.Geometry => ShaderType.GeometryShader,
                ShaderStage.TessellationControl => ShaderType.TessControlShader,
                ShaderStage.TessellationEvaluation => ShaderType.TessEvaluationShader,
                ShaderStage.Fragment => ShaderType.FragmentShader,
                ShaderStage.Compute => ShaderType.ComputeShader,
                _ => 0,
            };
        }

        public static DrawElementsType FromEnigmaIndex(IndexFormat indexFormat)
        {
            return indexFormat switch
            {
                IndexFormat.UShort => DrawElementsType.UnsignedShort,
                IndexFormat.UInt => DrawElementsType.UnsignedInt,
                _ => 0
            };
        }

        public static BufferTargetARB FromEnigmaBuffer(BufferUsage bufferUsage)
        {
            return bufferUsage switch
            {
                BufferUsage.VertexBuffer => BufferTargetARB.ArrayBuffer,
                BufferUsage.IndexBuffer => BufferTargetARB.ElementArrayBuffer,
                BufferUsage.UniformBuffer => BufferTargetARB.UniformBuffer,
                BufferUsage.StructuredBufferReadOnly => BufferTargetARB.CopyReadBuffer,
                BufferUsage.StructuredBufferReadWrite => BufferTargetARB.CopyWriteBuffer,
                BufferUsage.IndirectBuffer => BufferTargetARB.DrawIndirectBuffer,
                BufferUsage.Dynamic => throw new NotSupportedException("Dynamic buffers doesn't support in OpenGL"),
                BufferUsage.Staging => BufferTargetARB.ShaderStorageBuffer,
                _ => 0
            };
        }

        public static PrimitiveType FromEnigmaPrimtive(PrimitiveTopology primitiveTopology)
        {
            return primitiveTopology switch
            {
                PrimitiveTopology.TriangleList => PrimitiveType.Triangles,
                PrimitiveTopology.TriangleStrip => PrimitiveType.TriangleStrip,
                PrimitiveTopology.LineList => PrimitiveType.Lines,
                PrimitiveTopology.LineStrip => PrimitiveType.LineStrip,
                PrimitiveTopology.PointList => PrimitiveType.Points,
                _ => 0
            };
        }

        public static VertexAttribPointerType FromEnigmaVertex(VertexElementType vertexElement)
        {
            return vertexElement switch
            {
                VertexElementType.Byte => VertexAttribPointerType.UnsignedByte,
                VertexElementType.Float => VertexAttribPointerType.Float,
                VertexElementType.Int => VertexAttribPointerType.Int,
                VertexElementType.UInt => VertexAttribPointerType.UnsignedInt,
                VertexElementType.Short => VertexAttribPointerType.Short,
                VertexElementType.UShort => VertexAttribPointerType.UnsignedShort,
                VertexElementType.SByte => VertexAttribPointerType.Byte,
                VertexElementType.Half => VertexAttribPointerType.HalfFloat,
                _ => 0
            };
        }

        public static PolygonMode FromEnigmaPolygon(PolygonFillMode fillMode)
        {
            return fillMode switch
            {
                PolygonFillMode.Solid => PolygonMode.Fill,
                PolygonFillMode.Wireframe => PolygonMode.Line,
                _ => 0
            };
        }

        public static unsafe byte* ToGL(this string str)
        {
            int byteCount = System.Text.Encoding.UTF8.GetByteCount(str) + 1;
            byte* elementNamePtr = stackalloc byte[byteCount];
            fixed (char* charPtr = str)
            {
                int bytesWritten = System.Text.Encoding.UTF8.GetBytes(charPtr, str.Length, elementNamePtr, byteCount);
            }
            elementNamePtr[byteCount - 1] = 0; // Add null terminator.
            return elementNamePtr;
        }
    }
}
