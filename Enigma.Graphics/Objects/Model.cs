using System;
using System.Collections.Generic;
using System.Numerics;
using Assimp;
using Veldrid;
using Veldrid.Utilities;
using Veldrid.ImageSharp;
using AScene = Assimp.Scene;
using AMesh = Assimp.Mesh;

namespace Enigma.Graphics.Objects
{
    public class Model
    {
        private AssimpContext context;
        private AScene model;

        public Model()
        {
            context = new AssimpContext();
        }

        public void LoadModel(string filePath)
        {
            model = context.ImportFile(filePath);
        }

        public void ImportToScene(ref Scene scene)
        {
        }
    }
}
