using System;
using System.Collections.Generic;
using Silk.NET.OpenGL;

namespace Enigma.Graphics.Silk
{
    public class GlPipeline : Pipeline
    {
        public uint glCode;

        protected readonly GL gl;

        public GlPipeline(GL gl, IShader[] shaders, params ResourceLayout[] resources) : base(shaders, resources)
        {
            this.gl = gl;
            Create();
        }

        public GlPipeline(GL gl, PrimitiveTopology topology, PolygonFillMode fillMode, IShader[] shaders, params ResourceLayout[] resources)
            : base(topology, fillMode, shaders, resources)
        {
            this.gl = gl;
            Create();
        }

        private void Create()
        {
            glCode = gl.CreateProgram();
            int shadersLength = shaders.Length;
            uint[] shaderCodes = new uint[shadersLength];
            for (int i = 0; i < shadersLength; i++)
            {
                if (shaders[i] is GlShader gls)
                {
                    shaderCodes[i] = gls.glCode;
                    gl.AttachShader(glCode, gls.glCode);
                }
                else
                    throw new NotSupportedException(
                        $"{shaders[i]} cannot be added to {nameof(GlPipeline)} because it's not {nameof(GlShader)}");
            }
            gl.LinkProgram(glCode);

            // Remove individual shaders
            for (int i = 0; i < shadersLength; i++)
            {
                gl.DetachShader(glCode, shaderCodes[i]);
                gl.DeleteShader(shaderCodes[i]);
            }
        }
    }
}
