using System;
using System.Numerics;
using Mallos.Input;
using Enigma.Graphics.Objects;

namespace Enigma.Graphics.Sample
{
    public class CameraController
    {
        public Camera camera;

        public CameraController(Camera camera, Sdl.SdlWindow window)
        {
            this.camera = camera;
            camera.Position = -Vector3.UnitZ * 50;
            window.Input.KeyboardTracker.KeyDown += KeyboardTracker_KeyDown;
        }

        private void KeyboardTracker_KeyDown(object sender, KeyEventArgs e)
        {
            Console.WriteLine(camera.Position);
            Console.WriteLine(e.Key);
            if (e.Key == Keys.W)
                camera.Position += Vector3.UnitZ;
            if (e.Key == Keys.A)
                camera.Position += Vector3.UnitX;
            if (e.Key == Keys.S)
                camera.Position -= Vector3.UnitZ;
            if (e.Key == Keys.D)
                camera.Position -= Vector3.UnitX;
        }
    }
}
