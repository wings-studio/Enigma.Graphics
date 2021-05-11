using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enigma.Graphics.Shaders
{
    public static class ShaderStorage
    {
        public static readonly byte[] Color2DVertex, Color2DFragment;
        /// <summary>
        /// Name of all main functions of shaders
        /// </summary>
        public const string MainFunc = "main";

        static ShaderStorage()
        {
            Color2DFragment = Encoding.UTF8.GetBytes(@"#version 450
layout(location = 0) in vec4 fsin_Color;
layout(location = 0) out vec4 fsout_Color;
void main()
{
    fsout_Color = fsin_Color;
}");
            Color2DVertex = Encoding.UTF8.GetBytes(@"#version 450
layout(location = 0) in vec2 Position;
layout(location = 1) in vec4 Color;
layout(location = 0) out vec4 fsin_Color;
void main()
{
    gl_Position = vec4(Position, 0, 1);
    fsin_Color = Color;
}");
        }
    }
}
