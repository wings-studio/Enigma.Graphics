using Veldrid;

namespace Enigma.Graphics.Veldrid
{
    public static class VeldridUtil
    {
        public static GraphicsBackend FromEnigmaGraphicsAPI(GraphicsAPI graphicsAPI)
        {
            return graphicsAPI switch
            {
                GraphicsAPI.OpenGL => GraphicsBackend.OpenGL,
                GraphicsAPI.Direct3D11 => GraphicsBackend.Direct3D11,
                GraphicsAPI.Vulkan => GraphicsBackend.Vulkan,
                GraphicsAPI.OpenGLES => GraphicsBackend.OpenGLES,
                GraphicsAPI.Metal => GraphicsBackend.Metal,
                _ => throw new EnigmaVeldridException($"Veldrid doesn't support {graphicsAPI} graphics API")
            };
        }

        public static GraphicsAPI FromVeldridGraphicsBackend(GraphicsBackend graphicsBackend)
        {
            return graphicsBackend switch
            {
                GraphicsBackend.Direct3D11 => GraphicsAPI.Direct3D11,
                GraphicsBackend.Vulkan => GraphicsAPI.Vulkan,
                GraphicsBackend.OpenGL => GraphicsAPI.OpenGL,
                GraphicsBackend.Metal => GraphicsAPI.Metal,
                GraphicsBackend.OpenGLES => GraphicsAPI.OpenGLES,
                _ => GraphicsAPI.None,
            };
        }

        public static global::Veldrid.BufferUsage FromEnigmaBuffer(BufferUsage usage)
        {
            return usage switch
            {
                BufferUsage.VertexBuffer => global::Veldrid.BufferUsage.VertexBuffer,
                BufferUsage.IndexBuffer => global::Veldrid.BufferUsage.IndexBuffer,
                BufferUsage.UniformBuffer => global::Veldrid.BufferUsage.UniformBuffer,
                BufferUsage.StructuredBufferReadOnly => global::Veldrid.BufferUsage.StructuredBufferReadOnly,
                BufferUsage.StructuredBufferReadWrite => global::Veldrid.BufferUsage.StructuredBufferReadWrite,
                BufferUsage.IndirectBuffer => global::Veldrid.BufferUsage.IndirectBuffer,
                BufferUsage.Dynamic => global::Veldrid.BufferUsage.Dynamic,
                BufferUsage.Staging => global::Veldrid.BufferUsage.Staging,
                _ => 0,
            };
        }

        public static BufferUsage FromVeldridBuffer(global::Veldrid.BufferUsage bufferUsage)
        {
            return bufferUsage switch
            {
                global::Veldrid.BufferUsage.VertexBuffer => BufferUsage.VertexBuffer,
                global::Veldrid.BufferUsage.IndexBuffer => BufferUsage.IndexBuffer,
                global::Veldrid.BufferUsage.UniformBuffer => BufferUsage.UniformBuffer,
                global::Veldrid.BufferUsage.StructuredBufferReadOnly => BufferUsage.StructuredBufferReadOnly,
                global::Veldrid.BufferUsage.StructuredBufferReadWrite => BufferUsage.StructuredBufferReadWrite,
                global::Veldrid.BufferUsage.IndirectBuffer => BufferUsage.IndirectBuffer,
                global::Veldrid.BufferUsage.Dynamic => BufferUsage.Dynamic,
                global::Veldrid.BufferUsage.Staging => BufferUsage.Staging,
                _ => 0,
            };
        }

        public static global::Veldrid.IndexFormat FromEnigmaIndex(IndexFormat indexFormat)
        {
            return indexFormat switch
            {
                IndexFormat.UShort => global::Veldrid.IndexFormat.UInt16,
                IndexFormat.UInt => global::Veldrid.IndexFormat.UInt32,
                _ => 0
            };
        }
    }
}
