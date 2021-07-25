namespace Enigma.Graphics
{
    public class VertexElement
    {
        /// <summary>
        /// Size of element. Can be only: 1, 2, 3, 4
        /// </summary>
        public readonly int Size;
        public readonly string Name;
        public readonly VertexElementType Type;

        public VertexElement(int size, string name, VertexElementType type)
        {
            Size = size;
            Name = name;
            Type = type;
        }
    }
}
