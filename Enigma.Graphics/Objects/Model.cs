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

        public Model(System.IO.Stream fileStream) : this()
        {
            LoadModel(fileStream);
        }

        public void LoadModel(string filePath)
        {
            model = context.ImportFile(filePath);
            UpdateMeshes();
        }

        public void LoadModel(System.IO.Stream fileStream)
        {
            model = context.ImportFileFromStream(fileStream);
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
                    TexturedMesh mesh = new TexturedMesh(new AssimpMesh<VertexPositionTexture>(model.Meshes[i]))
                    {
                        TexturePath = model.Materials[model.Meshes[i].MaterialIndex].Name + ".jpg"
                    };
                    Meshes.Add(mesh);
                }
            }
        }

        public void ImportToScene<T>(T scene) where T : Scene
        {
            foreach (TexturedMesh mesh in Meshes)
                scene.Add(mesh);
        }

        public void ImportToRenderStage<T>(string stage, T renderer) where T : Renderer
        {
            foreach (TexturedMesh mesh in Meshes)
                renderer.Add(stage, mesh);
        }
    }
}
