using System;
using System.Numerics;
using Vortice.Mathematics;

namespace Enigma.Graphics
{
    public interface IGraphicsDevice : IDisposable
    {
        GraphicsAPI GraphicsAPI { get; set; }

        ResourceSet CreateResourceSet(ResourceLayout layout, params IResource[] resources);
        void SetResourceSet(int index, ResourceSet resourceSet);
        Pipeline CreatePipeline(IShader[] shaders, params ResourceLayout[] resources);
        Pipeline CreatePipeline(PrimitiveTopology topology, PolygonFillMode fillMode, IShader[] shaders, params ResourceLayout[] resources);
        void SetPipeline(Pipeline pipeline);


        IShader LoadShader(byte[] shader, ShaderStage stage);
        IShader LoadShader(string shader, ShaderStage stage);

        IBuffer CreateBuffer(int size, BufferUsage usage);
        void UpdateBuffer<T>(IBuffer buffer, T[] data, uint offsetInBytes = 0) where T : unmanaged;
        void UpdateBuffer<T>(IBuffer buffer, T data, uint offsetInBytes = 0) where T : unmanaged;
        void UpdateBuffer<T>(IBuffer buffer, ref T data, uint offsetInBytes = 0) where T : unmanaged;
        void UpdateBuffer(IBuffer buffer, IntPtr data, int sizeInBytes, uint offsetInBytes = 0);
        void UpdateBuffer<T>(IBuffer buffer, ReadOnlySpan<T> source, uint offsetInBytes = 0) where T : unmanaged;

        void SetIndexBuffer(IBuffer indexBuffer, IndexFormat format, uint offset = 0);
        /// <param name="index">The buffer slot.</param>
        void SetVertexBuffer(uint index, IBuffer vertexBuffer, uint offset = 0);

        /// <summary>
        /// Draws primitives from the currently-bound state in this CommandList. An index Buffer is not used.
        /// </summary>
        /// <param name="vertexCount">The number of vertices.</param>
        /// <param name="instanceCount">The number of instances.</param>
        /// <param name="vertexStart">The first vertex to use when drawing.</param>
        /// <param name="instanceStart">The starting instance value.</param>
        void Draw(uint vertexCount, uint instanceCount = 1, uint vertexStart = 0, uint instanceStart = 0);
        /// <summary>
        /// Draws indexed primitives from the currently-bound state
        /// </summary>
        /// <param name="indexCount">The number of indices.</param>
        /// <param name="instanceCount">The number of instances.</param>
        /// <param name="indexStart">The number of indices to skip in the active index buffer.</param>
        /// <param name="vertexOffset">The base vertex value, which is added to each index value read from the index buffer.</param>
        /// <param name="instanceStart">The starting instance value.</param>
        void DrawIndexed(uint indexCount, uint instanceCount = 1, uint indexStart = 0, int vertexOffset = 0, uint instanceStart = 0);
        //void DrawIndirect(IBuffer indirectBuffer, uint offset, uint drawCount, uint stride);
        //void DrawIndexedIndirect(IBuffer indirectBuffer, uint offset, uint drawCount, uint stride);

        void ClearColor(Color color);
        void Begin();
        void End();
    }

    public enum IndexFormat : byte
    {
        UShort,
        UInt
    }
}
