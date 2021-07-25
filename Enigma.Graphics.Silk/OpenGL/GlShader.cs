using System;
using System.Collections.Generic;
using Silk.NET.OpenGL;

namespace Enigma.Graphics.Silk.OpenGL
{
    public class GlShader : IShader
    {
        public ShaderStage Stage { get; set; }
        public uint glCode;

        protected readonly GL gl;

        public GlShader(GL Gl, ShaderStage stage)
        {
            gl = Gl;
            glCode = gl.CreateShader(GlUtil.FromEnigmaShader(stage));
            Stage = stage;
        }
        public GlShader(GL Gl, string source, ShaderStage stage) : this(Gl, stage)
        {
            LoadSources(source);
            CompileShader();
        }
        public GlShader(GL Gl, byte[] source, ShaderStage stage) : this(Gl, stage)
        {
            LoadByteCode(source);
            CompileShader();
        }

        public void LoadSources(string source)
        {
            gl.ShaderSource(glCode, source);
        }
        public unsafe void LoadByteCode(byte[] code)
        {
            fixed (byte* data = code)
            {
            }
        }
        public void CompileShader()
        {
            gl.CompileShader(glCode);
            string infoLog = gl.GetShaderInfoLog(glCode);
            if (!string.IsNullOrWhiteSpace(infoLog))
            {
                Console.WriteLine($"Error compiling shader {infoLog}");
            }
        }
    }
}
