using System.Numerics;
using Veldrid;

namespace Enigma.Graphics.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            using SampleApp app = new (Veldrid.StartupUtilities.VeldridStartup.GetPlatformDefaultBackend());

            app.Window.Title = $"Enigma Graphics Sample using {app.Scene.GraphicsDevice.BackendType} Graphics API";

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

            app.AddRenderable(quad1);
            app.AddRenderable(quad);

            app.Run();
        }
    }
}
