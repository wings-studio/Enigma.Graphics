using System;
using System.Numerics;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;
using Vortice.Mathematics;

namespace Enigma.Graphics.Veldrid
{
    public class VeldridDevice : IGraphicsDevice
    {
        private Sdl2Window window;
        private GraphicsDevice gd;
        private CommandList cl;
        private ResourceFactory factory;

        public GraphicsAPI GraphicsAPI 
        { 
            get => VeldridUtil.FromVeldridGraphicsBackend(gd.BackendType);
            set
            {
                gd = VeldridStartup.CreateGraphicsDevice(window, VeldridUtil.FromEnigmaGraphicsAPI(value));
                Init();
            }
        }

        public VeldridDevice()
        {
            VeldridStartup.CreateWindowAndGraphicsDevice(new WindowCreateInfo(), out window, out gd);
        }

        public VeldridDevice(GraphicsDevice graphicsDevice)
        {
            gd = graphicsDevice;
            Init();
        }

        public VeldridDevice(GraphicsBackend graphicsBackend)
        {
            gd = VeldridStartup.CreateGraphicsDevice(window, graphicsBackend);
            Init();
        }

        public VeldridDevice(GraphicsAPI graphicsAPI)
        {
            GraphicsAPI = graphicsAPI;
        }

        public void Begin()
        {
            cl.Begin();
        }

        public void ClearColor(Color4 color)
        {
            cl.ClearColorTarget(0, new RgbaFloat(color));
        }

        public IBuffer CreateBuffer(int size, BufferUsage usage)
        {
            throw new NotImplementedException();
        }

        public Pipeline CreatePipeline(IShader[] shaders, VertexElement[] vertexElements, params ResourceLayout[] resources)
        {
            throw new NotImplementedException();
        }

        public Pipeline CreatePipeline(PrimitiveTopology topology, PolygonFillMode fillMode, IShader[] shaders, VertexElement[] vertexElements, params ResourceLayout[] resources)
        {
            throw new NotImplementedException();
        }

        public ResourceSet CreateResourceSet(ResourceLayout layout, params IResource[] resources)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            cl.Dispose();
            gd.Dispose();
        }

        public void Draw(uint vertexCount, uint instanceCount = 1, uint vertexStart = 0, uint instanceStart = 0)
        {
            cl.Draw(vertexCount, instanceCount, vertexStart, instanceStart);
        }

        public void DrawIndexed(uint indexCount, uint instanceCount = 1, uint indexStart = 0, int vertexOffset = 0, uint instanceStart = 0)
        {
            cl.DrawIndexed(indexCount, instanceStart, indexStart, vertexOffset, instanceStart);
        }

        public void DrawIndexedIndirect(IBuffer indirectBuffer, uint offset, uint drawCount, uint stride)
        {
            throw new NotImplementedException();
        }

        public void DrawIndirect(IBuffer indirectBuffer, uint offset, uint drawCount, uint stride)
        {
            throw new NotImplementedException();
        }

        public void End()
        {
            cl.End();
            gd.SubmitCommands(cl);
            gd.WaitForIdle();
        }

        public IShader LoadShader(byte[] shader, ShaderStage stage)
        {
            throw new NotImplementedException();
        }

        public IShader LoadShader(string shader, ShaderStage stage)
        {
            throw new NotImplementedException();
        }

        public void SetIndexBuffer(IBuffer indexBuffer, IndexFormat format, uint offset = 0)
        {
            throw new NotImplementedException();
        }

        public void SetPipeline(Pipeline pipeline)
        {
            throw new NotImplementedException();
        }

        public void SetResourceSet(int index, ResourceSet resourceSet)
        {
            throw new NotImplementedException();
        }

        public void SetUniform1(int value)
        {
            throw new NotImplementedException();
        }

        public void SetUniform1(double value)
        {
            throw new NotImplementedException();
        }

        public void SetUniform1(float value)
        {
            throw new NotImplementedException();
        }

        public void SetUniform2(Vector2 value)
        {
            throw new NotImplementedException();
        }

        public void SetUniform3(Vector3 value)
        {
            throw new NotImplementedException();
        }

        public void SetUniform4(Vector4 value)
        {
            throw new NotImplementedException();
        }

        public void SetUniformBuffer(IBuffer buffer)
        {
            throw new NotImplementedException();
        }

        public void SetUniformMatrix3x2(Matrix3x2 matrix)
        {
            throw new NotImplementedException();
        }

        public void SetUniformMatrix4x4(Matrix4x4 matrix)
        {
            throw new NotImplementedException();
        }

        public void SetVertexBuffer(uint index, IBuffer vertexBuffer, uint offset = 0)
        {
            throw new NotImplementedException();
        }

        public void UpdateBuffer<T>(IBuffer buffer, T[] data, uint offsetInBytes = 0) where T : unmanaged
        {
            throw new NotImplementedException();
        }

        public void UpdateBuffer<T>(IBuffer buffer, T data, uint offsetInBytes = 0) where T : unmanaged
        {
            throw new NotImplementedException();
        }

        public void UpdateBuffer<T>(IBuffer buffer, ref T data, uint offsetInBytes = 0) where T : unmanaged
        {
            throw new NotImplementedException();
        }

        public void UpdateBuffer(IBuffer buffer, IntPtr data, int sizeInBytes, uint offsetInBytes = 0)
        {
            throw new NotImplementedException();
        }

        public void UpdateBuffer<T>(IBuffer buffer, ReadOnlySpan<T> source, uint offsetInBytes = 0) where T : unmanaged
        {
            throw new NotImplementedException();
        }

        private void Init()
        {
            factory = gd.ResourceFactory;
            cl = factory.CreateCommandList();
        }
    }
}
