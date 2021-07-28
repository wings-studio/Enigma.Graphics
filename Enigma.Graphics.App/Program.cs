using System.Numerics;
using Enigma.Graphics.Silk;

namespace Enigma.Graphics.App
{
    class Program
    {
        static void Main(string[] args)
        {
            Renderer renderer = new SilkRenderer();
            MeshScene<VertexColorPosition> scene = new MeshScene<VertexColorPosition>();
            Mesh<VertexColorPosition> mesh = new Mesh<VertexColorPosition>()
            {
                Vertices = new VertexColorPosition[]
                {
                    new VertexColorPosition(0.5f,  0.5f, 0.0f, 1, 0, 0, 1),
                    new VertexColorPosition(0.5f, -0.5f, 0.0f, 0, 0, 0, 1),
                    new VertexColorPosition(-0.5f, -0.5f, 0.0f, 0, 0, 1, 1),
                    new VertexColorPosition(-0.5f,  0.5f, 0.5f, 0, 0, 0, 1)
                },
                Indicies = new uint[]
                {
                    0, 1, 3,
                    1, 2, 3
                }
            };
            scene.Meshes.Add(mesh);
            renderer.Add("main", scene);
            renderer.Render();
        }
    }

    struct VertexColorPosition
    {
        public float X;
        public float Y;
        public float Z;

        public float R;
        public float G;
        public float B;
        public float A;

        public VertexColorPosition(float x, float y, float z, float r, float g, float b, float a)
        {
            X = x;
            Y = y;
            Z = z;
            R = r;
            G = g;
            B = b;
            A = a;
        }
    }
}
