using System;
using Silk.NET.OpenGL;
using System.Runtime.InteropServices;

namespace Enigma.Graphics.Silk
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
    }
}
