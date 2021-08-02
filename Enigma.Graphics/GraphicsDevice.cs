using System;
using System.Numerics;
using Vortice.Mathematics;

namespace Enigma.Graphics
{
    public abstract class GraphicsDevice : IGraphicsDevice
    {
        public virtual GraphicsAPI GraphicsAPI { get; set; }
        public Color4 ColorForClear { get; set; }

        private string resourceName;

        public virtual void Begin()
        {
            ClearColor(ColorForClear);
        }
        public abstract void ClearColor(Color4 color);
        public abstract IBuffer CreateBuffer(int size, BufferUsage usage);
        public abstract Pipeline CreatePipeline(IShader[] shaders, VertexElement[] vertexElements, params ResourceLayout[] resources);
        public abstract Pipeline CreatePipeline(PrimitiveTopology topology, PolygonFillMode fillMode, IShader[] shaders, VertexElement[] vertexElements, params ResourceLayout[] resources);
        public virtual ResourceLayout CreateResourceLayout(params ResourceElement[] elements)
            => new ResourceLayout(elements);
        public virtual ResourceSet CreateResourceSet(ResourceLayout layout, params IResource[] resources)
            => new ResourceSet(layout, resources);
        public abstract void Dispose();
        public abstract void Draw(uint vertexCount, uint instanceCount = 1, uint vertexStart = 0, uint instanceStart = 0);
        public abstract void DrawIndexed(uint indexCount, uint instanceCount = 1, uint indexStart = 0, int vertexOffset = 0, uint instanceStart = 0);
        public abstract void DrawIndexedIndirect(IBuffer indirectBuffer, uint offset, uint drawCount, uint stride);
        public abstract void DrawIndirect(IBuffer indirectBuffer, uint offset, uint drawCount, uint stride);
        public virtual void End() { }
        public abstract IShader LoadShader(byte[] shader, ShaderStage stage);
        public abstract IShader LoadShader(string shader, ShaderStage stage);
        public abstract void SetIndexBuffer(IBuffer indexBuffer, IndexFormat format, uint offset = 0);
        public abstract void SetPipeline(Pipeline pipeline);
        public void SetResourceSet(int index, ResourceSet resourceSet)
        {
            for (int i = 0; i < resourceSet.Resources.Length; i++)
            {
                resourceName = resourceSet.Layout.Elements[i].Name;
                resourceSet.Resources[i].SetResources(this);
            }
        }
        public void SetUniform1(int value) => SetUniform1(resourceName, value);
        public void SetUniform1(double value) => SetUniform1(resourceName, value);
        public void SetUniform1(float value) => SetUniform1(resourceName, value);
        public void SetUniform2(Vector2 value) => SetUniform2(resourceName, value);
        public void SetUniform3(Vector3 value) => SetUniform3(resourceName, value);
        public void SetUniform4(Vector4 value) => SetUniform4(resourceName, value);
        public void SetUniformBuffer(IBuffer buffer) => SetUniformBuffer(resourceName, buffer);
        public void SetUniformMatrix3x2(Matrix3x2 matrix) => SetUniformMatrix3x2(resourceName, matrix);
        public void SetUniformMatrix4x4(Matrix4x4 matrix) => SetUniformMatrix4x4(resourceName, matrix);
        public abstract void SetUniform1(string resourceName, int value);
        public abstract void SetUniform1(string resourceName, double value);
        public abstract void SetUniform1(string resourceName, float value);
        public abstract void SetUniform2(string resourceName, Vector2 value);
        public abstract void SetUniform3(string resourceName, Vector3 value);
        public abstract void SetUniform4(string resourceName, Vector4 value);
        public abstract void SetUniformBuffer(string resourceName, IBuffer buffer);
        public abstract void SetUniformMatrix3x2(string resourceName, Matrix3x2 matrix);
        public abstract void SetUniformMatrix4x4(string resourceName, Matrix4x4 matrix);
        public abstract void SetVertexBuffer(uint index, IBuffer vertexBuffer, uint offset = 0);
        public abstract void UpdateBuffer<T>(IBuffer buffer, T[] data, uint offsetInBytes = 0) where T : unmanaged;
        public abstract void UpdateBuffer<T>(IBuffer buffer, T data, uint offsetInBytes = 0) where T : unmanaged;
        public abstract void UpdateBuffer<T>(IBuffer buffer, ref T data, uint offsetInBytes = 0) where T : unmanaged;
        public abstract void UpdateBuffer(IBuffer buffer, IntPtr data, int sizeInBytes, uint offsetInBytes = 0);
        public abstract void UpdateBuffer<T>(IBuffer buffer, ReadOnlySpan<T> source, uint offsetInBytes = 0) where T : unmanaged;
    }
}
