namespace Enigma.Graphics.App
{
    static class Program
    {
        static void Main(string[] args)
        {
            Renderer renderer =
                //new Silk.SilkRenderer();
                new Veldrid.VeldridRenderer();
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
            scene.AddRenderTask<MeshRenderTask>();
            renderer.Add("main", scene);
            renderer.ClearColor = Vortice.Mathematics.Color4.LightSkyBlue;
            renderer.Render();
        }
    }
}
