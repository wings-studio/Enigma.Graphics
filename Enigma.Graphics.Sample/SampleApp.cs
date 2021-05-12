using Veldrid;
using Enigma.Graphics.Sdl;

namespace Enigma.Graphics.Sample
{
    public class SampleApp : SdlApplication
    {
        public SampleApp(GraphicsBackend backend = GraphicsBackend.Vulkan) : base(backend) { }
    }
}
