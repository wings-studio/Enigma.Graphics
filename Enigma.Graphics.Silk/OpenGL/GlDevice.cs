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
        private uint ibOffset = 0;

        public GlDevice(IWindow window)
        {
            Window = window;
            Window.Load += OnLoad;
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
            // https://github.com/mellinoe/veldrid/blob/7c248955fb4666a6df177932d44add206636959f/src/Veldrid/OpenGL/OpenGLCommandExecutor.cs#L123
            if (instanceCount == 1 && instanceStart == 0)
            {
                Gl.DrawArrays(primitive, (int)vertexStart, vertexCount);
            }
            else
            {
                if (instanceStart == 0)
                {
                    Gl.DrawArraysInstanced(primitive, (int)vertexStart, vertexCount, instanceCount);
                }
                else
                {
                    Gl.DrawArraysInstancedBaseInstance(primitive, (int)vertexStart, vertexCount, instanceCount, instanceStart);
                }
            }
        }

        public unsafe void DrawIndexed(uint indexCount, uint instanceCount = 1, uint indexStart = 0, int vertexOffset = 0, uint instanceStart = 0)
        {
            // https://github.com/mellinoe/veldrid/blob/7c248955fb4666a6df177932d44add206636959f/src/Veldrid/OpenGL/OpenGLCommandExecutor.cs#L143
            uint indexSize = indexType == DrawElementsType.UnsignedShort ? 2u : 4u;
            void* indices = (void*)((indexStart * indexSize) + ibOffset);
            if (instanceCount == 1 && instanceStart == 0)
            {
                if (vertexOffset == 0)
                {
                    Gl.DrawElements(primitive, indexCount, indexType, indices);
                }
                else
                {
                    Gl.DrawElementsBaseVertex(primitive, indexCount, indexType, indices, vertexOffset);
                }
            }
            else
            {
                if (instanceStart > 0)
                {
                    Gl.DrawElementsInstancedBaseVertexBaseInstance(
                        primitive,
                        indexCount,
                        indexType,
                        indices,
                        instanceCount,
                        vertexOffset,
                        instanceStart);
                }
                else if (vertexOffset == 0)
                {
                    Gl.DrawElementsInstanced(primitive, indexCount, indexType, indices, instanceCount);
                }
                else
                {
                    Gl.DrawElementsInstancedBaseVertex(
                        primitive,
                        indexCount,
                        indexType,
                        indices,
                        instanceCount,
                        vertexOffset);
                }
            }
        }

        public void End()
        {
        }

        public void SetIndexBuffer(IBuffer indexBuffer, IndexFormat format, uint offset = 0)
        {
            indexType = GlUtil.FromEnigmaIndex(format);
            ibOffset = offset;
            if (indexBuffer is GlBuffer glib)
                glib.Bind();
            else
                throw new SilkGlException($"Buffer must has type {nameof(GlBuffer)}");
        }

        public void SetPipeline(Pipeline pipeline)
        {
            primitive = GlUtil.FromEnigmaPrimtive(pipeline.Topology);
            fillMode = GlUtil.FromEnigmaPolygon(pipeline.FillMode);
            if (pipeline is GlPipeline glp)
            {
                Gl.UseProgram(glp.GlCode);
                this.pipeline = glp;
            }
            else
            {
                throw new SilkGlException($"Pipeline must be {nameof(GlPipeline)} type");
            }
        }

        public unsafe void SetResourceSet(int index, ResourceSet resourceSet)
        {
            uint program = pipeline.GlCode;
            for (int i = 0; i < resourceSet.Resources.Length; i++)
            {
                ResourceElement re = resourceSet.Layout.Elements[i];
                switch (re.Kind)
                {
                    case ResourceKind.UniformBuffer:
                        {
                            IBuffer buf = resourceSet.Resources[i].GetBuffer(this, BufferUsage.UniformBuffer);
                            if (buf is GlBuffer glb)
                            {
                                uint blockIndex = Gl.GetUniformBlockIndex(program, re.Name.ToGL());
                                Gl.GetActiveUniformBlock(program, blockIndex, GLEnum.UniformBlockDataSize, out int blockSize);
                                Gl.UniformBlockBinding(program, blockIndex, (uint)i);
                                Gl.BindBufferRange(GlUtil.FromEnigmaBuffer(glb.Usage), blockIndex, glb.GlCode, 0, (nuint)blockSize);
                                break;
                            }
                            else
                                throw new SilkGlException($"Resource generates buffer with wrong type (it's not {nameof(GlBuffer)})");
                        }

                    case ResourceKind.StructuredBufferReadOnly:
                        break;
                    case ResourceKind.StructuredBufferReadWrite:
                        break;
                    case ResourceKind.TextureReadOnly:
                        break;
                    case ResourceKind.TextureReadWrite:
                        break;
                    case ResourceKind.Sampler:
                        break;
                    default:
                        break;
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
