namespace Enigma.Graphics
{
    public interface IResource
    {
        /// <summary>
        /// Set resources. Calls universal methods of <see cref="IGraphicsDevice"/>
        /// like <see cref="IGraphicsDevice.SetUniform1(float)"/> or other Set*()
        /// </summary>
        void SetResources(IGraphicsDevice graphicsDevice);
    }
}
