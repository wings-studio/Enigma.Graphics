using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Veldrid;

namespace Enigma.Graphics.Sample
{
    public sealed class CameraController : IUpdateable
    {
        public Camera Camera { private set; get; }

        public float Speed = 10f;
        private bool _mousePressed;
        private Vector2 _mousePressedPos;

        public CameraController(Camera camera)
        {
            Camera = camera;
        }

        public void Update(float deltaTime)
        {
            float sprintFactor = Input.GetKey(Key.ControlLeft)
                ? 0.1f
                : Input.Input.GetKey(Key.ShiftLeft)
                    ? 2.5f
                    : 1f;
            Vector3 motionDir = Vector3.Zero;
            if (Input.Input.GetKey(Key.A))
            {
                motionDir += -Vector3.UnitX;
            }
            if (Input.Input.GetKey(Key.D))
            {
                motionDir += Vector3.UnitX;
            }
            if (Input.Input.GetKey(Key.W))
            {
                motionDir += -Vector3.UnitZ;
            }
            if (Input.Input.GetKey(Key.S))
            {
                motionDir += Vector3.UnitZ;
            }
            if (Input.Input.GetKey(Key.Q))
            {
                motionDir += -Vector3.UnitY;
            }
            if (Input.Input.GetKey(Key.E))
            {
                motionDir += Vector3.UnitY;
            }

            if (motionDir != Vector3.Zero)
            {
                Quaternion lookRotation = Quaternion.CreateFromYawPitchRoll(Camera.Yaw, Camera.Pitch, 0f);
                motionDir = Vector3.Transform(motionDir, lookRotation);
                Camera.Position += motionDir * Speed * sprintFactor * deltaTime;
            }

            if (Input.Input.GetMouseButton(MouseButton.Left) || Input.Input.GetMouseButton(MouseButton.Right))
            {
                if (!_mousePressed)
                {
                    _mousePressed = true;
                    _mousePressedPos = Input.Input.MousePosition;
                    //Sdl2Native.SDL_ShowCursor(0);
                    //Sdl2Native.SDL_SetWindowGrab(Window.SdlWindowHandle, true);
                }
                Vector2 mouseDelta = _mousePressedPos - Input.Input.MousePosition;
                //Sdl2Native.SDL_WarpMouseInWindow(_window.SdlWindowHandle, (int)_mousePressedPos.X, (int)_mousePressedPos.Y);
                Camera.Yaw += mouseDelta.X * 0.002f;
                Camera.Pitch += mouseDelta.Y * 0.002f;
            }
            else if (_mousePressed)
            {
                //Sdl2Native.SDL_WarpMouseInWindow(_window.SdlWindowHandle, (int)_mousePressedPos.X, (int)_mousePressedPos.Y);
                //Sdl2Native.SDL_SetWindowGrab(_window.SdlWindowHandle, false);
                //Sdl2Native.SDL_ShowCursor(1);
                _mousePressed = false;
            }

            Camera.Pitch = Math.Clamp(Camera.Pitch, -1.55f, 1.55f);
            Camera.UpdateViewMatrix();
        }
    }
}
