using System;
using Silk.NET.OpenGL;

namespace Enigma.Graphics.Silk.OpenGL
{
    public class GlShader : IShader
    {
        public ShaderStage Stage { get; set; }
        public uint GlCode;

        protected readonly GL gl;

        public GlShader(GL Gl, ShaderStage stage)
        {
            gl = Gl;
            GlCode = gl.CreateShader(GlUtil.FromEnigmaShader(stage));
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
            gl.ShaderSource(GlCode, source);
        }
        public unsafe void LoadByteCode(byte[] code)
        {
            fixed (byte* data = code)
            {
                throw new NotImplementedException();
            }
        }
        public void CompileShader()
        {
            gl.CompileShader(GlCode);
            string infoLog = gl.GetShaderInfoLog(GlCode);
            if (!string.IsNullOrWhiteSpace(infoLog))
            {
                Console.WriteLine($"Error compiling OpenGL shader {infoLog}");
            }
        }

        public void Dispose()
        {
            gl.DeleteShader(GlCode);
        }
    }
}
