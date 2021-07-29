using System;
using Silk.NET.OpenGL;

namespace Enigma.Graphics.Silk.OpenGL
{
    public class GlPipeline : Pipeline
    {
        public uint GlCode;

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
            GlCode = gl.CreateProgram();
            #region Link shaders
            int shadersLength = Shaders.Length;
            uint[] shaderCodes = new uint[shadersLength];
            for (int i = 0; i < shadersLength; i++)
            {
                if (Shaders[i] is GlShader gls)
                {
                    shaderCodes[i] = gls.GlCode;
                    gl.AttachShader(GlCode, gls.GlCode);
                }
                else
                    throw new NotSupportedException(
                        $"{Shaders[i]} cannot be added to {nameof(GlPipeline)} because it's not {nameof(GlShader)}");
            }
            #endregion
            #region Bind vertex elements
            for (uint i = 0; i < VertexElements.Length; i++)
            {
                // https://github.com/mellinoe/veldrid/blob/7c248955fb4666a6df177932d44add206636959f/src/Veldrid/OpenGL/OpenGLPipeline.cs#L125
                gl.BindAttribLocation(GlCode, i, VertexElements[i].Name);
                gl.EnableVertexAttribArray(i);
            }
            #endregion
            gl.LinkProgram(GlCode);
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
