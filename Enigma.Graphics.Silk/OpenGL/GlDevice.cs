using System;
using System.Runtime.InteropServices;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Vortice.Mathematics;

namespace Enigma.Graphics.Silk.OpenGL
{
    public sealed class GlDevice : IGraphicsDevice
    {
        public GL Gl;
        public IWindow Window;

        private GlPipeline pipeline;
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
                    throw new SilkGlException($"{nameof(GlDevice)} supports only {nameof(GraphicsAPI.OpenGL)}");
            }
        }

        public void Begin()
        {
            ClearColor(new Color(0));
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
                return new GlBuffer(Gl, size, usage);
        }

        public ResourceSet CreateResourceSet(ResourceLayout layout, params IResource[] resources)
        {
            return new ResourceSet(layout, resources);
        }

        public void Dispose()
        {
            Gl.Dispose();
        }

        public void Draw(uint vertexCount, uint instanceCount = 1, uint vertexStart = 0, uint instanceStart = 0)
        {
            throw new NotImplementedException();
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
            primitive = GlUtil.FromEnigmaPrimtive(pipeline.Topology);
            fillMode = GlUtil.FromEnigmaPolygon(pipeline.FillMode);
            if (pipeline is GlPipeline glp)
            {
                Gl.UseProgram(glp.glCode);
                this.pipeline = glp;
            }
            else
            {
                throw new SilkGlException($"Pipeline must be {nameof(GlPipeline)} type");
            }
        }

        public unsafe void SetResourceSet(int index, ResourceSet resourceSet)
        {
            for (int i = 0; i < resourceSet.Resources.Length; i++)
            {
                ResourceElement re = resourceSet.Layout.Elements[i];
                if (re.Kind == ResourceKind.UniformBuffer)
                {
                    int uniform = Gl.GetUniformLocation(pipeline.glCode, re.Name.ToGL());
                }
            }
        }

        public void SetVertexBuffer(uint index, IBuffer vertexBuffer, uint offset = 0)
        {
            if (vertexBuffer is GlVertexArray gvb)
                Gl.BindVertexArray(gvb.GlArrayCode);
            else
                throw new SilkGlException($"{nameof(vertexBuffer)} must be {nameof(GlVertexArray)} type");
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

        public Pipeline CreatePipeline(IShader[] shaders, VertexElement[] vertexElements, params ResourceLayout[] resources)
            => new GlPipeline(Gl, shaders, vertexElements, resources);

        public Pipeline CreatePipeline(PrimitiveTopology topology, PolygonFillMode fillMode, IShader[] shaders, VertexElement[] vertexElements, params ResourceLayout[] resources)
            => new GlPipeline(Gl, topology, fillMode, shaders, vertexElements, resources);

        public void DrawIndirect(IBuffer indirectBuffer, uint offset, uint drawCount, uint stride)
        {
            throw new NotImplementedException();
        }

        public void DrawIndexedIndirect(IBuffer indirectBuffer, uint offset, uint drawCount, uint stride)
        {
            throw new NotImplementedException();
        }
    }
}
