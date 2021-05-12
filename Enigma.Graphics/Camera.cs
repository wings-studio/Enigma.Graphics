using System;
using System.Numerics;
using System.Runtime.InteropServices;
using Veldrid;

namespace Enigma.Graphics
{
    public class Camera : IUpdateable
    {
        private float _fov = 1f;
        private float _near = 1f;
        private float _far = 1000f;

        private Matrix4x4 _viewMatrix;
        private Matrix4x4 _projectionMatrix;

        private Vector3 _position = new (0, 3, 0);
        private Vector3 _lookDirection = new (0, -.3f, -1f);

        private float _yaw;
        private float _pitch;

        private GraphicsDevice _gd;
        private bool _useReverseDepth;
        private float _windowWidth;
        private float _windowHeight;
        private IWindow _window;

        public event Action<Matrix4x4> ProjectionChanged;
        public event Action<Matrix4x4> ViewChanged;

        public Matrix4x4 ViewMatrix => _viewMatrix;
        public Matrix4x4 ProjectionMatrix => _projectionMatrix;

        public Vector3 Position { get => _position; set { _position = value; UpdateViewMatrix(); } }
        public Vector3 LookDirection => _lookDirection;

        public float FarDistance => _far;

        public float FieldOfView => _fov;
        public float NearDistance => _near;

        public float AspectRatio => _windowWidth / _windowHeight;

        public float Yaw { get => _yaw; set { _yaw = value; UpdateViewMatrix(); } }
        public float Pitch { get => _pitch; set { _pitch = value; UpdateViewMatrix(); } }

        public Camera(GraphicsDevice gd, IWindow window)
        {
            _gd = gd;
            _useReverseDepth = gd.IsDepthRangeZeroToOne;
            _window = window;
            _windowWidth = window.Width;
            _windowHeight = window.Height;
            UpdatePerspectiveMatrix();
            UpdateViewMatrix();
        }

        public void UpdateBackend(GraphicsDevice gd, IWindow window)
        {
            _gd = gd;
            _useReverseDepth = gd.IsDepthRangeZeroToOne;
            _window = window;
            _windowWidth = window.Width;
            _windowHeight = window.Height;
            UpdatePerspectiveMatrix();
        }

        public void WindowResized(float width, float height)
        {
            _windowWidth = width;
            _windowHeight = height;
            UpdatePerspectiveMatrix();
        }

        private void UpdatePerspectiveMatrix()
        {
            _projectionMatrix = Util.CreatePerspective(
                _gd,
                _useReverseDepth,
                _fov,
                _windowWidth / _windowHeight,
                _near,
                _far);
            ProjectionChanged?.Invoke(_projectionMatrix);
        }

        private void UpdateViewMatrix()
        {
            Quaternion lookRotation = Quaternion.CreateFromYawPitchRoll(Yaw, Pitch, 0f);
            Vector3 lookDir = Vector3.Transform(-Vector3.UnitZ, lookRotation);
            _lookDirection = lookDir;
            _viewMatrix = Matrix4x4.CreateLookAt(_position, _position + _lookDirection, Vector3.UnitY);
            ViewChanged?.Invoke(_viewMatrix);
        }

        public CameraInfo GetCameraInfo() => new CameraInfo
        {
            CameraPosition_WorldSpace = _position,
            CameraLookDirection = _lookDirection
        };

        public void Update(float deltaTime)
        {
            throw new NotImplementedException();
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CameraInfo
    {
        public Vector3 CameraPosition_WorldSpace;
        private float _padding1;
        public Vector3 CameraLookDirection;
        private float _padding2;
    }
}
