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
        internal Sdl2Window Window;

        private global::Veldrid.GraphicsDevice Gd
        {
            get => gd;
            set
            {
                gd = value;
                factory = gd.ResourceFactory;
                cl = factory.CreateCommandList();
            }
        }

        private CommandList cl;
        private ResourceFactory factory;
        private global::Veldrid.GraphicsDevice gd;

        public GraphicsAPI GraphicsAPI 
        { 
            get => VeldridUtil.FromVeldridGraphicsBackend(Gd.BackendType);
            set => Gd = VeldridStartup.CreateGraphicsDevice(Window, VeldridUtil.FromEnigmaGraphicsAPI(value));
        }
        public Color4 ColorForClear { get; set; }

        public VeldridDevice()
        {
            VeldridStartup.CreateWindowAndGraphicsDevice(new WindowCreateInfo(), out Window, out global::Veldrid.GraphicsDevice _gd);
            Gd = _gd; // gd is property so we can't write in method upper: out gd, but we need to run setter of gd so we assign it here
        }

        public VeldridDevice(global::Veldrid.GraphicsDevice graphicsDevice)
        {
            Gd = graphicsDevice;
        }

        public VeldridDevice(GraphicsBackend graphicsBackend)
        {
            Gd = VeldridStartup.CreateGraphicsDevice(Window, graphicsBackend);
        }

        public VeldridDevice(GraphicsAPI graphicsAPI)
        {
            GraphicsAPI = graphicsAPI;
        }

        public void Begin()
        {
            cl.Begin();
            ClearColor(ColorForClear);
        }

        public void ClearColor(Color4 color)
            => cl.ClearColorTarget(0, new RgbaFloat(color));

        public IBuffer CreateBuffer(int size, BufferUsage usage)
            => new VeldridBuffer(factory.CreateBuffer(new BufferDescription((uint)size, VeldridUtil.FromEnigmaBuffer(usage))));

        public Pipeline CreatePipeline(IShader[] shaders, VertexElement[] vertexElements, params ResourceLayout[] resources)
            => new VeldridPipeline(factory, shaders, vertexElements, resources);

        public Pipeline CreatePipeline(PrimitiveTopology topology, PolygonFillMode fillMode, IShader[] shaders, VertexElement[] vertexElements, params ResourceLayout[] resources)
            => new VeldridPipeline(factory, topology, fillMode, shaders, vertexElements, resources);

        public ResourceSet CreateResourceSet(ResourceLayout layout, params IResource[] resources)
        {

            throw new NotImplementedException();
        }

        public void Dispose()
        {
            cl.Dispose();
            Gd.Dispose();
        }

        public void Draw(uint vertexCount, uint instanceCount = 1, uint vertexStart = 0, uint instanceStart = 0)
            => cl.Draw(vertexCount, instanceCount, vertexStart, instanceStart);

        public void DrawIndexed(uint indexCount, uint instanceCount = 1, uint indexStart = 0, int vertexOffset = 0, uint instanceStart = 0)
            => cl.DrawIndexed(indexCount, instanceStart, indexStart, vertexOffset, instanceStart);

        public void DrawIndexedIndirect(IBuffer indirectBuffer, uint offset, uint drawCount, uint stride)
            => cl.DrawIndexedIndirect(GetDeviceBuffer(indirectBuffer), offset, drawCount, stride);

        public void DrawIndirect(IBuffer indirectBuffer, uint offset, uint drawCount, uint stride) 
            => cl.DrawIndirect(GetDeviceBuffer(indirectBuffer), offset, drawCount, stride);

        public void End()
        {
            cl.End();
            Gd.SubmitCommands(cl);
            Gd.WaitForIdle();
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
            => cl.SetIndexBuffer(GetDeviceBuffer(indexBuffer), VeldridUtil.FromEnigmaIndex(format), offset);

        public void SetPipeline(Pipeline pipeline)
        {
            if (pipeline is VeldridPipeline vdPipeline)
                cl.SetPipeline(vdPipeline.VdPipeline);
            else
                throw new EnigmaVeldridException($"Pipeline is not {nameof(VeldridPipeline)} so it cannot be using");
        }

        public void SetResourceSet(int index, ResourceSet resourceSet)
        {
            throw new NotImplementedException();
        }

        public void SetUniform1(int value) { }

        public void SetUniform1(double value) { }

        public void SetUniform1(float value) { }

        public void SetUniform2(Vector2 value) { }

        public void SetUniform3(Vector3 value) { }

        public void SetUniform4(Vector4 value) { }

        public void SetUniformBuffer(IBuffer buffer) { }

        public void SetUniformMatrix3x2(Matrix3x2 matrix) { }

        public void SetUniformMatrix4x4(Matrix4x4 matrix) { }

        public void SetVertexBuffer(uint index, IBuffer vertexBuffer, uint offset = 0) 
            => cl.SetVertexBuffer(index, GetDeviceBuffer(vertexBuffer), offset);

        public void UpdateBuffer<T>(IBuffer buffer, T[] data, uint offsetInBytes = 0) where T : unmanaged
        {
            cl.UpdateBuffer(GetDeviceBuffer(buffer), offsetInBytes, data);
        }

        public void UpdateBuffer<T>(IBuffer buffer, T data, uint offsetInBytes = 0) where T : unmanaged
        {
            cl.UpdateBuffer(GetDeviceBuffer(buffer), offsetInBytes, data);
        }

        public void UpdateBuffer<T>(IBuffer buffer, ref T data, uint offsetInBytes = 0) where T : unmanaged
        {
            cl.UpdateBuffer(GetDeviceBuffer(buffer), offsetInBytes, ref data);
        }

        public void UpdateBuffer(IBuffer buffer, IntPtr data, int sizeInBytes, uint offsetInBytes = 0)
        {
            cl.UpdateBuffer(GetDeviceBuffer(buffer), offsetInBytes, data, (uint)sizeInBytes);
        }

        public void UpdateBuffer<T>(IBuffer buffer, ReadOnlySpan<T> source, uint offsetInBytes = 0) where T : unmanaged
        {
            cl.UpdateBuffer(GetDeviceBuffer(buffer), offsetInBytes, source);
        }

        public ResourceLayout CreateResourceLayout(params ResourceElement[] elements)
            => new VeldridResourceLayout(factory, elements);

        private static DeviceBuffer GetDeviceBuffer(IBuffer buffer)
        {
            if (buffer is VeldridBuffer vBuffer)
                return vBuffer.DeviceBuffer;
            else
                throw new EnigmaVeldridException($"Buffer is not {nameof(VeldridBuffer)} so it cannot be using in this renderer");
        }
    }
}
