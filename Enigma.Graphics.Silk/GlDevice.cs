using System;
using System.Runtime.InteropServices;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Vortice.Mathematics;

namespace Enigma.Graphics.Silk
{
    public class GlDevice : IGraphicsDevice
    {
        public GL Gl;
        public IWindow Window;

        private PrimitiveType primitive;
        private DrawElementsType indexType;
        private PolygonMode fillMode;

        public GlDevice()
        {
            Window = global::Silk.NET.Windowing.Window.Create(WindowOptions.Default);
            Window.Load += OnLoad;

            Window.Run();
        }

        private void OnLoad()
        {
            Gl = GL.GetApi(Window);
        }

        public GraphicsAPI GraphicsAPI
        {
            get => GraphicsAPI.OpenGL;
            set
            {
                if (value != GraphicsAPI.OpenGL)
                    throw new ArgumentException($"{nameof(GlDevice)} supports only {nameof(GraphicsAPI.OpenGL)}");
            }
        }

        public void Begin()
        {
        }

        public void ClearColor(Color color)
        {
            Gl.ClearColor(color.R, color.G, color.B, color.A);
        }

        public IBuffer CreateBuffer(int size, BufferUsage usage)
        {
            if (usage == BufferUsage.VertexBuffer)
                return new GlVertexArray(Gl, size);
            else
            {
                GlBuffer buffer = new GlBuffer(Gl, size, usage);
                return buffer;
            }
        }

        public ResourceSet CreateResourceSet(ResourceLayout layout, params IResource[] resources)
        {
            return new ResourceSet() { resource = layout, resources = resources };
        }

        public void Dispose()
        {
            Gl.Dispose();
        }

        public void Draw(uint vertexCount, uint instanceCount = 1, uint vertexStart = 0, uint instanceStart = 0)
        {
            //pass
        }

        public unsafe void DrawIndexed(uint indexCount, uint instanceCount = 1, uint indexStart = 0, int vertexOffset = 0, uint instanceStart = 0)
        {
            Gl.DrawElements(primitive, indexCount, indexType, null);
        }

        public void End()
        {
        }

        public void SetIndexBuffer(IBuffer indexBuffer, IndexFormat format, uint offset = 0)
        {
            indexType = GlUtil.FromEnigmaIndex(format);
        }

        public void SetPipeline(Pipeline pipeline)
        {
            primitive = GlUtil.FromEnigmaPrimtive(pipeline.topology);
            if (pipeline is GlPipeline glp)
            {
                Gl.UseProgram(glp.glCode);
            }
        }

        public unsafe void SetResourceSet(int index, ResourceSet resourceSet)
        {
            foreach (IResource resource in resourceSet.resources)
            {
                Gl.VertexAttribPointer((uint)index, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), null);
            }
            Gl.EnableVertexAttribArray((uint)index);
        }

        public void SetVertexBuffer(uint index, IBuffer vertexBuffer, uint offset = 0)
        {
            if (vertexBuffer is GlVertexArray gvb)
                Gl.BindVertexArray(gvb.GlArrayCode);
            else
                throw new ArgumentException($"{nameof(vertexBuffer)} must be {nameof(GlVertexArray)} type");
        }

        public unsafe void UpdateBuffer<T>(IBuffer buffer, T[] data, uint offsetInBytes = 0) where T : unmanaged
        {
            if (buffer is GlBuffer gb)
            {
                gb.Bind();
                fixed (void* v = &data[0])
                {
                    Gl.BufferData(GlUtil.FromEnigmaBuffer(buffer.Usage), (nuint)(data.Length * sizeof(T)), v, BufferUsageARB.StaticDraw);
                }
            }
        }

        public unsafe void UpdateBuffer<T>(IBuffer buffer, T data, uint offsetInBytes = 0) where T : unmanaged
        {
            if (buffer is GlBuffer gb)
            {
                gb.Bind();
                void* v = &data;
                Gl.BufferData(GlUtil.FromEnigmaBuffer(buffer.Usage), (nuint)Marshal.SizeOf(data), v, BufferUsageARB.StaticDraw);
            }
        }

        public void UpdateBuffer<T>(IBuffer buffer, ref T data, uint offsetInBytes = 0) where T : unmanaged
        {
            UpdateBuffer(buffer, data, offsetInBytes);
        }

        public unsafe void UpdateBuffer(IBuffer buffer, IntPtr data, int sizeInBytes, uint offsetInBytes = 0)
        {
            if (buffer is GlBuffer gb)
            {
                gb.Bind();
                void* v = data.ToPointer();
                Gl.BufferData(GlUtil.FromEnigmaBuffer(buffer.Usage), (nuint)Marshal.SizeOf(data), v, BufferUsageARB.StaticDraw);
            }
        }

        public void UpdateBuffer<T>(IBuffer buffer, ReadOnlySpan<T> source, uint offsetInBytes = 0) where T : unmanaged
        {
            throw new NotImplementedException();
        }

        public IShader LoadShader(byte[] shader, ShaderStage stage) => new GlShader(Gl, shader, stage);

        public IShader LoadShader(string shader, ShaderStage stage) => new GlShader(Gl, shader, stage);

        public Pipeline CreatePipeline(IShader[] shaders, params ResourceLayout[] resources)
            => new GlPipeline(Gl, shaders, resources);

        public Pipeline CreatePipeline(PrimitiveTopology topology, PolygonFillMode fillMode, IShader[] shaders, params ResourceLayout[] resources)
            => new GlPipeline(Gl, topology, fillMode, shaders, resources);
    }
}
