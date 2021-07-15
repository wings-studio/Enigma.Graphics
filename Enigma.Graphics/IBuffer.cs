using System;
using System.Numerics;

namespace Enigma.Graphics
{
    public interface IBuffer : IDisposable, IResource
    {
        uint Size { set; get; }
        BufferUsage Usage { set; get; }
    }
}
