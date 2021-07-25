using System;
using System.Collections.Generic;
using Silk.NET.OpenGL;

namespace Enigma.Graphics.Silk
{
    public class GlPipeline : Pipeline
    {
        public uint glCode;

        protected readonly GL gl;

        public GlPipeline(GL gl, IShader[] shaders, VertexElement[] vertexElements, params ResourceLayout[] resources) 
            : base(shaders, vertexElements, resources)
        {
            this.gl = gl;
            Create();
        }

        public GlPipeline(GL gl, PrimitiveTopology topology, PolygonFillMode fillMode, IShader[] shaders, VertexElement[] vertexElements, params ResourceLayout[] resources)
            : base(topology, fillMode, shaders, vertexElements, resources)
        {
            this.gl = gl;
            Create();
        }

        private unsafe void Create()
        {
            glCode = gl.CreateProgram();
            #region Link shaders
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
            #endregion
            #region Bind vertex elements
            for (int i = 0; i < vertexElements.Length; i++)
            {
                // https://github.com/mellinoe/veldrid/blob/7c248955fb4666a6df177932d44add206636959f/src/Veldrid/OpenGL/OpenGLPipeline.cs#L125
                gl.BindAttribLocation(glCode, (uint)i, vertexElements[i].Name.ToGL());
            }
            #endregion
            gl.LinkProgram(glCode);
            //#region Remove individual shaders
            //for (int i = 0; i < shadersLength; i++)
            //{
            //    gl.DetachShader(glCode, shaderCodes[i]);
            //    gl.DeleteShader(shaderCodes[i]);
            //}
            //#endregion
        }
    }
}
