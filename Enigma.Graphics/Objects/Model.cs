using System.Collections.Generic;
using Assimp;
using Veldrid;
using AScene = Assimp.Scene;
using BBox = Veldrid.Utilities.BoundingBox;

namespace Enigma.Graphics.Objects
{
    public class Model : RenderObject
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

        public override BBox BoundingBox
        {
            get
            {
                BBox b = new BBox();
                foreach (TexturedMesh mesh in Meshes)
                    b = BBox.Combine(b, mesh.BoundingBox);
                return b;
            }
        }

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

        public override void CreateDeviceObjects(GraphicsDevice gd, CommandList cl)
        {
            foreach (TexturedMesh mesh in Meshes)
                mesh.CreateDeviceObjects(gd, cl);
        }

        public override void Dispose()
        {
            foreach (TexturedMesh mesh in Meshes)
                mesh.Dispose();
        }

        public override void Render(CommandList cl, Camera camera)
        {
            foreach (TexturedMesh mesh in Meshes)
                mesh.Render(cl, camera);
        }

        public override void UpdatePerFrameResources(CommandList cl)
        {
            foreach (TexturedMesh mesh in Meshes)
                mesh.UpdatePerFrameResources(cl);
        }
    }
}
