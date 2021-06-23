using Veldrid;
using Enigma.Sdl;
using Enigma.Graphics.Objects;
using System.Diagnostics;
using System.Numerics;
using Mallos.Input;
using System;

namespace Enigma.Graphics.Sample
{
    class Program
    {
        const string RENDER_STAGE = "Main";
        const float ANGLE = 30;

        static SdlWindow window;
        static Renderer renderer;
        static float deltaSeconds = 1 / 60;

        static void Init()
        {
            AssetHelper.SetShadersPath();
            window = new SdlWindow();
            window.Title = "Enigma Graphics Application";
            renderer = new Renderer(window, true);
            renderer.ClearColor = RgbaFloat.Black;
            TexturedMesh.TextureColor = RgbaByte.Blue;
            Renderer.Storage = new RealtimeStorage();
            ObjectScene scene = new ObjectScene(renderer.GraphicsDevice, renderer.Window);
            camera = scene.Camera;
            SetupCamera();
            Model model = new Model(AssetHelper.GetPath("plechovy_sud.FBX"));
            scene.Add(model);
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
                deltaSeconds = sw.Elapsed.Seconds - lastSeconds;
                window.Update(deltaSeconds);
                renderer.RenderAll(deltaSeconds);
            }
            sw.Stop();
            renderer.Dispose();
        }

        static Camera camera;

        static Vector2 lastMousePosition;

        public static void SetupCamera()
        {
            camera.Position = new Vector3(14, 45, 0);
            // next you need rotate to right to see model (bug right now)
            window.Input.KeyboardTracker.KeyDown += KeyboardTracker_KeyDown;
            //window.Input.MouseTracker.Move += MouseTracker_Move;
            window.Input.Mouse.GetPosition(out int x, out int y);
            lastMousePosition = new Vector2(x, y);
        }

        private static void MouseTracker_Move(object sender, MouseEventArgs e)
        {
            Vector2 mouseDelta = lastMousePosition - e.State.Position;
            camera.Rotate(new Vector3(lastMousePosition - e.State.Position, 0) * deltaSeconds);
        }

        private static void KeyboardTracker_KeyDown(object sender, KeyEventArgs e)
        {
            Console.WriteLine(deltaSeconds);
            Console.WriteLine(camera.Yaw + " " + camera.Pitch);
            Console.WriteLine(camera.Position);
            Console.WriteLine(camera.Rotation);
            Console.WriteLine(e.Key);

            // Movement
            if (e.Key == Keys.S)
                camera.Position += Vector3.UnitY;
            if (e.Key == Keys.A)
                camera.Position += Vector3.UnitX;
            if (e.Key == Keys.Space)
                camera.Position += Vector3.UnitZ;
            if (e.Key == Keys.W)
                camera.Position -= Vector3.UnitY;
            if (e.Key == Keys.D)
                camera.Position -= Vector3.UnitX;
            if (e.Key == Keys.LeftControl)
                camera.Position -= Vector3.UnitZ;

            // Rotation
            if (e.Key == Keys.Up)
                camera.Rotate(Vector3.UnitZ, ANGLE);
            else if (e.Key == Keys.Down)
                camera.Rotate(Vector3.UnitZ, -ANGLE);
            if (e.Key == Keys.Right)
                camera.Rotate(Vector3.UnitY, ANGLE);
            else if (e.Key == Keys.Left)
                camera.Rotate(Vector3.UnitY, -ANGLE);
        }

        static void Main(string[] args)
        {
            Init();
            Run();
        }
    }
}
