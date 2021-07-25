namespace Enigma.Graphics
{
    public class ResourceSet
    {
        public ResourceLayout Layout;
        public IResource[] Resources;

        public ResourceSet(ResourceLayout layout, params IResource[] resources)
        {
            Layout = layout;
            Resources = resources;
        }
    }
}
