namespace Enigma.Graphics
{
    public class ResourceElement
    {
        public string name;
        public ResourceKind kind;
        public ShaderStage stage;

        public ResourceElement(string name, ResourceKind kind, ShaderStage stage)
        {
            this.name = name;
            this.kind = kind;
            this.stage = stage;
        }
    }
}
