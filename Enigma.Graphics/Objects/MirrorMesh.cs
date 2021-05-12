using System.Numerics;

namespace Enigma.Graphics.Objects
{
    internal class MirrorMesh
    {
        public static Plane Plane { get; set; } = new Plane(Vector3.UnitY, 0);
    }
}
