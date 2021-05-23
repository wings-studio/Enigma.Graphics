using System;
using System.Collections.Generic;
using System.Numerics;
using Assimp;
using Veldrid;
using Veldrid.Utilities;
using Veldrid.ImageSharp;

namespace Enigma.Graphics.Objects
{
    public class Model : CullRenderable
    {
        private Application app;
        private AssimpContext assimp;
        private Assimp.Scene model;

        public Model(ref Application application)
        {
            assimp = new AssimpContext();
            app = application;
        }

        public override Veldrid.Utilities.BoundingBox BoundingBox => throw new NotImplementedException();

        public override RenderPasses RenderPasses => throw new NotImplementedException();

        public override void CreateDeviceObjects(GraphicsDevice gd, CommandList cl, SceneContext sc)
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            throw new NotImplementedException();
        }

        public override RenderOrderKey GetRenderOrderKey(Vector3 cameraPosition)
        {
            throw new NotImplementedException();
        }

        public void LoadModel(string filePath)
        {
            LoadModel(assimp.ImportFile(filePath));
        }

        public override void Render(GraphicsDevice gd, CommandList cl, SceneContext sc, RenderPasses renderPass)
        {
            throw new NotImplementedException();
        }

        public override void UpdatePerFrameResources(GraphicsDevice gd, CommandList cl, SceneContext sc)
        {
            throw new NotImplementedException();
        }

        private void LoadModel(Assimp.Scene model)
        {
            foreach (Assimp.Mesh mesh in model.Meshes)
            {
                MeshData data = new AssimpMesh(mesh);
                TextureSlot mainTexture = model.Materials[mesh.MaterialIndex].TextureDiffuse;
                var texture = app.LoadTexture(mainTexture.FilePath, false);
            }
            this.model = model;
        }
    }
}
