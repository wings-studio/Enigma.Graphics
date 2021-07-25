namespace Enigma.Graphics
{
    public class ResourceElement
    {
        public readonly string Name;
        public readonly ResourceKind Kind;
        public readonly ShaderStage Stage;

        public ResourceElement(string name, ResourceKind kind, ShaderStage stage)
        {
            Name = name;
            Kind = kind;
            Stage = stage;
        }
    }
}
