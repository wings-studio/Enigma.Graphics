using System;
using System.Numerics;

namespace Enigma.Graphics
{
    public interface IGraphicsDevice : IDisposable
    {
        GraphicsAPI GraphicsAPI { get; set; }

        #region Resources, shaders and pipelines
        ResourceSet CreateResourceSet(ResourceLayout layout, params IResource[] resources);
        void SetResourceSet(int index, ResourceSet resourceSet);
        Pipeline CreatePipeline(IShader[] shaders, VertexElement[] vertexElements, params ResourceLayout[] resources);
        Pipeline CreatePipeline(PrimitiveTopology topology, PolygonFillMode fillMode, IShader[] shaders, VertexElement[] vertexElements, params ResourceLayout[] resources);
        void SetPipeline(Pipeline pipeline);
        IShader LoadShader(byte[] shader, ShaderStage stage);
        IShader LoadShader(string shader, ShaderStage stage);
        #endregion

        #region Buffer's methods
        IBuffer CreateBuffer(int size, BufferUsage usage);
        void UpdateBuffer<T>(IBuffer buffer, T[] data, uint offsetInBytes = 0) where T : unmanaged;
        void UpdateBuffer<T>(IBuffer buffer, T data, uint offsetInBytes = 0) where T : unmanaged;
        void UpdateBuffer<T>(IBuffer buffer, ref T data, uint offsetInBytes = 0) where T : unmanaged;
        void UpdateBuffer(IBuffer buffer, IntPtr data, int sizeInBytes, uint offsetInBytes = 0);
        void UpdateBuffer<T>(IBuffer buffer, ReadOnlySpan<T> source, uint offsetInBytes = 0) where T : unmanaged;
        void SetIndexBuffer(IBuffer indexBuffer, IndexFormat format, uint offset = 0);
        /// <param name="index">The buffer slot.</param>
        void SetVertexBuffer(uint index, IBuffer vertexBuffer, uint offset = 0);
        #endregion

        #region Draw methods
        /// <summary>
        /// Draws primitives . An index Buffer is not used.
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
        void DrawIndirect(IBuffer indirectBuffer, uint offset, uint drawCount, uint stride);
        void DrawIndexedIndirect(IBuffer indirectBuffer, uint offset, uint drawCount, uint stride);
        #endregion

        #region Resource binding methods
        void SetUniform1(int value);
        void SetUniform1(double value);
        void SetUniform1(float value);
        void SetUniform2(Vector2 value);
        void SetUniform3(Vector3 value);
        void SetUniform4(Vector4 value);
        void SetUniformMatrix4x4(Matrix4x4 matrix);
        void SetUniformMatrix3x2(Matrix3x2 matrix);
        void SetUniformBuffer(IBuffer buffer);
        #endregion

        void ClearColor(Vortice.Mathematics.Color4 color);
        void Begin();
        void End();
    }
}
