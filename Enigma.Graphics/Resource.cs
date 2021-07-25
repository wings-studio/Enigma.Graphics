namespace Enigma.Graphics
{
    public class Resource<T> : IResource where T : unmanaged
    {
        public T Value;

        public Resource(T value)
        {
            Value = value;
        }
    }
}
