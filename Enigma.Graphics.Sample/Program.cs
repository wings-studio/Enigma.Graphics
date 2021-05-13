using System.Numerics;
using Veldrid;

namespace Enigma.Graphics.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            using SampleApp app = new (Veldrid.StartupUtilities.VeldridStartup.GetPlatformDefaultBackend());

            app.Window.Title = $"Enigma Graphics Sample using {app.GraphicsDevice.BackendType} Graphics API";

            app.Run();
        }
    }
}
