using System.Numerics;
using Enigma.Graphics.Silk;

namespace Enigma.Graphics.App
{
    class Program
    {
        static void Main(string[] args)
        {
            Renderer renderer = new SilkRenderer();
            MeshScene<Vector3> scene = new MeshScene<Vector3>();
            Mesh<Vector3> mesh = new Mesh<Vector3>()
            {
                Vertices = new Vector3[]
                {
                    new Vector3(0.5f, 0.5f, 0.0f),
                    new Vector3(0.5f, -0.5f, 0.0f),
                    new Vector3(-0.5f, -0.5f, 0.0f),
                    new Vector3(-0.5f, 0.5f, 0.5f)
                },
                Indicies = new uint[]
                {
                    0, 1, 3,
                    1, 2, 3
                }
            };
            scene.meshes.Add(mesh);
            renderer.Add("main", scene);
            renderer.CreateResources();
            while (!System.Console.KeyAvailable)
            {
                renderer.BeginFrame();
                renderer.Render();
                renderer.EndFrame();
            }
        }
    }
}
