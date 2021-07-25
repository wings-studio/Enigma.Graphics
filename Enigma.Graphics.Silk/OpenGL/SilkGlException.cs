using System;
using System.Runtime.Serialization;

namespace Enigma.Graphics.Silk.OpenGL
{
    public class SilkGlException : Exception
    {
        public SilkGlException()
        {
        }

        public SilkGlException(string message) : base(message)
        {
        }

        public SilkGlException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SilkGlException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
