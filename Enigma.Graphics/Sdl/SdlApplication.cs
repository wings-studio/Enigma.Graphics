using Veldrid;

namespace Enigma.Graphics.Sdl
{
    public class SdlApplication : Application
    {
        public SdlApplication(GraphicsBackend backend = GraphicsBackend.Vulkan) : base(backend) { }

        protected override void CreateAllDeviceObjects(CommandList cl)
        {
        }

        protected override void DestroyAllDeviceObjects()
        {
        }

        protected override void InitWindow()
        {
            Window = new SdlWindow();
        }
    }
}
