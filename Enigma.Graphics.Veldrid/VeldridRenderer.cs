namespace Enigma.Graphics.Veldrid
{
    public class VeldridRenderer : Renderer
    {
        public VeldridRenderer()
        {
            GraphicsDevice = new VeldridDevice();
        }

        public VeldridRenderer(GraphicsAPI graphicsAPI)
        {
            GraphicsDevice = new VeldridDevice(graphicsAPI);
        }

    }
}
