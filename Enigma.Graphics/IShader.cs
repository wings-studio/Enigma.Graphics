namespace Enigma.Graphics
{
    public interface IShader : System.IDisposable
    {
        ShaderStage Stage { get; set; }
    }
}
