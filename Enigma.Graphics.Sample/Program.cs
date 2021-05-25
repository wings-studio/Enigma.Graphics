using System.Numerics;
using Veldrid;
using Enigma.Sdl;

namespace Enigma.Graphics.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            SdlWindow window = new SdlWindow();
            Renderer renderer = new Renderer(window, true);
            renderer.AddRenderStage("Main");
        }
    }
}
