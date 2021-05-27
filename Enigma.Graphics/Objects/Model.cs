using System;
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
        }

        public Model(string filePath) : this()
        {
            LoadModel(filePath);
        }

        public void LoadModel(string filePath)
        {
            model = context.ImportFile(filePath);
        }

        public IEnumerable<TexturedMesh> Meshes
        {
            get
            {
                if (model.HasMeshes)
                {
                    for (int i = 0; i < model.MeshCount; i++)
                    {
                        TexturedMesh mesh = new TexturedMesh(new AssimpMesh(model.Meshes[i]));
                        mesh.TexturePath =
                            System.Reflection.Assembly.GetExecutingAssembly().Location + model.Materials[model.Meshes[i].MaterialIndex].Name;
                        yield return mesh;
                    }
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
