namespace Enigma.Graphics
{
    public class ResourceLayout
    {
        public readonly ResourceElement[] Elements;

        public ResourceLayout(params ResourceElement[] elements)
        {
            Elements = elements;
        }
    }
}
