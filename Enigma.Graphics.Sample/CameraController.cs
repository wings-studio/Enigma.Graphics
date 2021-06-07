using System;
using System.Numerics;
using Mallos.Input;
using Enigma.Graphics.Objects;

namespace Enigma.Graphics.Sample
{
    public class CameraController
    {
        public Camera camera;

        private Vector2 lastMousePosition;

        public CameraController(Camera camera, Sdl.SdlWindow window)
        {
            this.camera = camera;
            camera.Position = new Vector3(0, 0, -100f);
            window.Input.KeyboardTracker.KeyDown += KeyboardTracker_KeyDown;
            window.Input.MouseTracker.Move += MouseTracker_Move;
            window.Input.Mouse.GetPosition(out int x, out int y);
            lastMousePosition = new Vector2(x, y);
        }

        private void MouseTracker_Move(object sender, MouseEventArgs e)
        {
            camera.Rotate(new Vector3(lastMousePosition - e.State.Position, 0));
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
            if (e.Key == Keys.Space)
                camera.Position += Vector3.UnitY;
            if (e.Key == Keys.LeftControl)
                camera.Position -= Vector3.UnitY;
        }
    }
}
