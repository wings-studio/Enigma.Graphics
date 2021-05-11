using System.Runtime.InteropServices;
using System.Numerics;
using Veldrid;
using System.Linq;

namespace Enigma.Graphics.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            SampleApp app;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                app = new SampleApp(GraphicsBackend.Direct3D11);
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                app = new SampleApp(GraphicsBackend.Metal);
            else
                app = new SampleApp(args.Contains("-vulkan") ? GraphicsBackend.Vulkan : GraphicsBackend.OpenGL);

            app.Window.Title = $"Enigma Graphics Sample using {app.GraphicsDevice.BackendType} Graphics API";

            Quad quad = new (
                new VertexPositionColor(new Vector2(0, 1), RgbaFloat.Red),
                new VertexPositionColor(new Vector2(.25f, 1), RgbaFloat.Green),
                new VertexPositionColor(new Vector2(0, .75f), RgbaFloat.Blue),
                new VertexPositionColor(new Vector2(.25f, .75f), RgbaFloat.Yellow)),
                quad1 = new (
                    new VertexPositionColor(new Vector2(-.75f, .75f), RgbaFloat.Cyan),
                new VertexPositionColor(new Vector2(.75f, .75f), RgbaFloat.Orange),
                new VertexPositionColor(new Vector2(-.75f, -.75f), RgbaFloat.Pink),
                new VertexPositionColor(new Vector2(.75f, -.75f), RgbaFloat.CornflowerBlue));

            app.AddDrawable(quad1);
            app.AddDrawable(quad);

            app.Run();
            app.Dispose();
        }
    }
}
