namespace Enigma.Graphics
{
    public enum ResourceKind : byte
    {
        /// <summary>
        /// Uniform contains one number (like double or int)
        /// </summary>
        Uniform1,
        /// <summary>
        /// Uniform contains two numbers (like Vector2)
        /// </summary>
        Uniform2,
        Uniform3,
        Uniform4,
        UniformMat3x2,
        UniformMat4,
        UniformBuffer,
        StructuredBufferReadOnly,
        StructuredBufferReadWrite,
        TextureReadOnly,
        TextureReadWrite,
        Sampler,
    }
}
