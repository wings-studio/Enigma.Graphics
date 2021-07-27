using System;
using System.Runtime.InteropServices;

namespace Enigma.Graphics
{
    public class Resource<T> : IResource where T : unmanaged
    {
        public T Value;
        public unsafe uint Size => (uint)sizeof(T);
        public IntPtr Data
        {
            get
            {
                IntPtr data = Marshal.AllocHGlobal((int)Size);
                Marshal.StructureToPtr(Value, data, false);
                return data;
            }
        }

        public Resource(T value)
        {
            Value = value;
        }
    }
}
