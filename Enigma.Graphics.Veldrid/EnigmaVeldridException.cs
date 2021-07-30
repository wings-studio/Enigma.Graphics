using System;
using System.Runtime.Serialization;

namespace Enigma.Graphics.Veldrid
{
    public class EnigmaVeldridException : Exception
    {
        public EnigmaVeldridException()
        {
        }

        public EnigmaVeldridException(string message) : base(message)
        {
        }

        public EnigmaVeldridException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected EnigmaVeldridException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
