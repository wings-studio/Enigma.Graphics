using System;
using System.Collections.Generic;
using System.Numerics;

namespace Enigma.Graphics.Objects
{
    public interface IVertexInfo
    {
        void SetVertex(Vector3 vertexCoord);
        uint SizeInBytes();
    }
}
