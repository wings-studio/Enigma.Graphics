namespace Enigma.Graphics.Veldrid
{
    public class VeldridRenderer : Renderer
    {
        public VeldridRenderer()
        {
            SetupVeldridDevice(new VeldridDevice());
        }

        public VeldridRenderer(GraphicsAPI graphicsAPI)
        {
            SetupVeldridDevice(new VeldridDevice(graphicsAPI));
        }

        private void SetupVeldridDevice(VeldridDevice veldridDevice)
        {
            veldridDevice.Window.Closed += () => IsRunning = false;
            GraphicsDevice = veldridDevice;
        }
    }
}
