using System.Collections.Generic;
using Assimp;
using AScene = Assimp.Scene;

namespace Enigma.Graphics.Objects
{
    public class Model
    {
        private readonly AssimpContext context;
        private AScene model;

        public Model()
        {
            context = new AssimpContext();
            Meshes = new List<TexturedMesh>();
        }

        public Model(string filePath) : this()
        {
            LoadModel(filePath);
        }

        public void LoadModel(string filePath)
        {
            model = context.ImportFile(filePath);
            UpdateMeshes();
        }

        public List<TexturedMesh> Meshes { get; private set; }

        private void UpdateMeshes()
        {
            if (model.HasMeshes)
            {
                Meshes.Clear();
                for (int i = 0; i < model.MeshCount; i++)
                {
                    TexturedMesh mesh = new TexturedMesh(new AssimpMesh(model.Meshes[i]))
                    {
                        TexturePath =
                        System.IO.Path.Combine(
                            System.Reflection.Assembly.GetExecutingAssembly().Location,
                            model.Materials[model.Meshes[i].MaterialIndex].Name)
                    };
                    Meshes.Add(mesh);
                }
            }
        }

        public void ImportToScene(ref Scene scene)
        {
            foreach (TexturedMesh mesh in Meshes)
                scene.Add(mesh);
        }

        public void ImportToRenderStage(string stage, Renderer renderer)
        {
            foreach (TexturedMesh mesh in Meshes)
                renderer.Add(stage, mesh);
        }
    }
}
