using System;
using System.Numerics;
using System.Runtime.InteropServices;
using Veldrid;

namespace Enigma.Graphics
{
    public class DirectionalLight
    {
        public Transform Transform { get; } = new Transform();

        public Vector3 Direction => Transform.Forward;

        public RgbaFloat Color { get; set; } = RgbaFloat.White;

        public DirectionalLight()
        {
            Vector3 lightDir = Vector3.Normalize(new Vector3(0.15f, -1f, -0.15f));
            Transform.Rotation = Util.FromToRotation(-Vector3.UnitZ, lightDir);
        }

        internal DirectionalLightInfo GetInfo()
        {
            return new DirectionalLightInfo { Direction = Transform.Forward, Color = Color.ToVector4() };
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct DirectionalLightInfo
    {
        public Vector3 Direction;
        private float _padding;
        public Vector4 Color;
    }
}
