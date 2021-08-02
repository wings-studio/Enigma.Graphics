using System;
using System.Numerics;
using System.Runtime.InteropServices;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using Vortice.Mathematics;

namespace Enigma.Graphics.Silk.OpenGL
{
    public sealed class GlDevice : GraphicsDevice
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
            ColorForClear = Color4.SkyBlue;
        }

        private void OnLoad()
        {
            Gl = GL.GetApi(Window);
        }

        public override GraphicsAPI GraphicsAPI
        {
            get => GraphicsAPI.OpenGL;
            set
            {
                if (value != GraphicsAPI.OpenGL)
                    throw new SilkGlException($"{nameof(GlDevice)} supports only {nameof(GraphicsAPI.OpenGL)}");
            }
        }

        public override void Begin()
        {
            base.Begin();
            Gl.Clear(ClearBufferMask.ColorBufferBit);
        }

        public override void ClearColor(Color4 color)
        {
            Gl.ClearColor((System.Drawing.Color)color);
        }

        public override IBuffer CreateBuffer(int size, BufferUsage usage)
        {
            if (usage == BufferUsage.VertexBuffer)
                return new GlVertexArray(Gl, size);
            else
                return new GlBuffer(Gl, size, usage);
        }

        public override void Dispose()
        {
            Gl.Dispose();
        }

        public override void Draw(uint vertexCount, uint instanceCount = 1, uint vertexStart = 0, uint instanceStart = 0)
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

        public unsafe override void DrawIndexed(uint indexCount, uint instanceCount = 1, uint indexStart = 0, int vertexOffset = 0, uint instanceStart = 0)
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

        public override void SetIndexBuffer(IBuffer indexBuffer, IndexFormat format, uint offset = 0)
        {
            indexType = GlUtil.FromEnigmaIndex(format);
            ibOffset = offset;
            if (indexBuffer is GlBuffer glib)
                glib.Bind();
            else
                throw new SilkGlException($"Buffer must has type {nameof(GlBuffer)}");
        }

        public override void SetPipeline(Pipeline pipeline)
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

        public override void SetVertexBuffer(uint index, IBuffer vertexBuffer, uint offset = 0)
        {
            if (vertexBuffer is GlVertexArray gvb)
                Gl.BindVertexArray(gvb.GlArrayCode);
            else
                throw new SilkGlException($"{nameof(vertexBuffer)} must be {nameof(GlVertexArray)} type");
        }

        public unsafe override void UpdateBuffer<T>(IBuffer buffer, T[] data, uint offsetInBytes = 0)
        {
            if (buffer is GlBuffer gb)
            {
                gb.Bind();
                fixed (void* v = &data[0])
                {
                    Gl.BufferData(GlUtil.FromEnigmaBuffer(buffer.Usage), (nuint)Util.Sizeof(data), v, BufferUsageARB.StaticDraw);
                }
            }
        }

        public unsafe override void UpdateBuffer<T>(IBuffer buffer, T data, uint offsetInBytes = 0)
        {
            if (buffer is GlBuffer gb)
            {
                gb.Bind();
                void* v = &data;
                Gl.BufferData(GlUtil.FromEnigmaBuffer(buffer.Usage), (nuint)Marshal.SizeOf(data), v, BufferUsageARB.StaticDraw);
            }
        }

        public override void UpdateBuffer<T>(IBuffer buffer, ref T data, uint offsetInBytes = 0)
        {
            UpdateBuffer(buffer, data, offsetInBytes);
        }

        public unsafe override void UpdateBuffer(IBuffer buffer, IntPtr data, int sizeInBytes, uint offsetInBytes = 0)
        {
            if (buffer is GlBuffer gb)
            {
                gb.Bind();
                void* v = data.ToPointer();
                Gl.BufferData(GlUtil.FromEnigmaBuffer(buffer.Usage), (nuint)Marshal.SizeOf(data), v, BufferUsageARB.StaticDraw);
            }
        }

        public override void UpdateBuffer<T>(IBuffer buffer, ReadOnlySpan<T> source, uint offsetInBytes = 0)
        {
            throw new NotImplementedException();
        }

        public override IShader LoadShader(byte[] shader, ShaderStage stage) => new GlShader(Gl, shader, stage);

        public override IShader LoadShader(string shader, ShaderStage stage) => new GlShader(Gl, shader, stage);

        public override Pipeline CreatePipeline(IShader[] shaders, VertexElement[] vertexElements, params ResourceLayout[] resources)
            => new GlPipeline(Gl, shaders, vertexElements, resources);

        public override Pipeline CreatePipeline(PrimitiveTopology topology, PolygonFillMode fillMode, IShader[] shaders, VertexElement[] vertexElements, params ResourceLayout[] resources)
            => new GlPipeline(Gl, topology, fillMode, shaders, vertexElements, resources);

        public override void DrawIndirect(IBuffer indirectBuffer, uint offset, uint drawCount, uint stride)
        {
            throw new NotImplementedException();
        }

        public override void DrawIndexedIndirect(IBuffer indirectBuffer, uint offset, uint drawCount, uint stride)
        {
            throw new NotImplementedException();
        }

        public override void SetUniform1(string resourceName, int value) => Gl.Uniform1(GetLocation(resourceName), value);

        public override void SetUniform1(string resourceName, double value) => Gl.Uniform1(GetLocation(resourceName), value);

        public override void SetUniform1(string resourceName, float value) => Gl.Uniform1(GetLocation(resourceName), value);

        public override void SetUniform2(string resourceName, Vector2 value) => Gl.Uniform2(GetLocation(resourceName), value);

        public override void SetUniform3(string resourceName, Vector3 value) => Gl.Uniform3(GetLocation(resourceName), value);

        public override void SetUniform4(string resourceName, Vector4 value) => Gl.Uniform4(GetLocation(resourceName), value);

        public override void SetUniformMatrix4x4(string resourceName, Matrix4x4 matrix) 
            => Gl.UniformMatrix4(GetLocation(resourceName), transpose: false, new ReadOnlySpan<float>(new float[] {
                matrix.M11, matrix.M12, matrix.M13, matrix.M14,
                matrix.M21, matrix.M22, matrix.M23, matrix.M24,
                matrix.M31, matrix.M32, matrix.M33, matrix.M34,
                matrix.M41, matrix.M42, matrix.M43, matrix.M44
            }));

        public override void SetUniformMatrix3x2(string resourceName, Matrix3x2 matrix)
            => Gl.UniformMatrix3x2(GetLocation(resourceName), transpose: false, new ReadOnlySpan<float>(new float[] {
                matrix.M11, matrix.M12,
                matrix.M21, matrix.M22,
                matrix.M31, matrix.M32
            }));

        public override void SetUniformBuffer(string resourceName, IBuffer buffer)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get location of uniform with name <param cref="resourceName"/>
        /// </summary>
        private unsafe int GetLocation(string resourceName)
        {
            int location = Gl.GetUniformLocation(pipeline.GlCode, resourceName);
            if (location == -1)
                throw new SilkGlException($"Uniform location with name {resourceName} not found");
            else
                return location;
        }
    }
}
