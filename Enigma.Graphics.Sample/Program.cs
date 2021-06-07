using Veldrid;
using Enigma.Sdl;
using Enigma.Graphics.Objects;
using System.Diagnostics;

namespace Enigma.Graphics.Sample
{
    class Program
    {
        const string RENDER_STAGE = "Main";

        static SdlWindow window;
        static Renderer renderer;

        static void Init()
        {
            AssetHelper.SetShadersPath();
            window = new SdlWindow();
            window.Title = "Enigma Graphics Application";
            renderer = new Renderer(window, true);
            renderer.ClearColor = RgbaFloat.Red;
            Renderer.Storage = new RealtimeStorage();
            ObjectScene scene = new ObjectScene(renderer.GraphicsDevice, renderer.Window);
            CameraController cc = new CameraController(scene.Camera, window);
            Model model = new Model(AssetHelper.GetPath("plechovy_sud.FBX"));
            model.ImportToScene(scene);
            renderer.AddRenderStage(RENDER_STAGE, scene);
            renderer.Init();
        }

        static void Run()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();
            float lastSeconds = 0;
            while (window.Exists)
            {
                float deltaSeconds = sw.Elapsed.Seconds - lastSeconds;
                window.Update(deltaSeconds);
                renderer.RenderAll(deltaSeconds);
            }
            sw.Stop();
            renderer.Dispose();
        }

        static void Main(string[] args)
        {
            Init();
            Run();
        }
    }
}
