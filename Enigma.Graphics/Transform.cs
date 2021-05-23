using System;
using System.Numerics;

namespace Enigma.Graphics
{
    public class Transform
    {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; } = Quaternion.Identity;
        public Vector3 Scale { get; set; } = Vector3.One;

        public Vector3 Forward => Vector3.Transform(-Vector3.UnitZ, Rotation);

        public Matrix4x4 GetTransformMatrix()
        {
            return Matrix4x4.CreateScale(Scale)
                * Matrix4x4.CreateFromQuaternion(Rotation)
                * Matrix4x4.CreateTranslation(Position);
        }
    }
}
