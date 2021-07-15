using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace Enigma.Graphics
{
    public static class Util
    {
        public static unsafe int Sizeof<T>(T[] array) where T : unmanaged => array.Length * sizeof(T);

        public static IShader LoadShader(string fileName, IGraphicsDevice gd, ShaderStage stage)
            => gd.LoadShader(File.ReadAllText(fileName), stage);
    }
}
