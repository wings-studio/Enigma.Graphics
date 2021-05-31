using System;
using System.IO;

namespace Enigma.Graphics.Sample
{
    public static class AssetHelper
    {
        private static readonly string s_assetRoot = "..\\..\\..\\Assets";

        public static string GetPath(string assetPath)
        {
            return Path.Combine(s_assetRoot, assetPath);
        }

        public static void SetShadersPath()
        {
            Shaders.ShaderHelper.ShadersFolder = "..\\..\\..\\..\\Enigma.Graphics\\Shaders";
        }
    }
}
