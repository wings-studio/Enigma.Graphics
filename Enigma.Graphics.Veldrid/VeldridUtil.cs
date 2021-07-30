using System;
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
    }
}
