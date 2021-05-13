using Veldrid;
using Enigma.Graphics.Sdl;
using System.Numerics;
using Veldrid.Utilities;
using Veldrid.ImageSharp;
using System.IO;
using Enigma.Graphics.Objects;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using System;

namespace Enigma.Graphics.Sample
{
    public class SampleApp : SdlApplication
    {
        public static Skybox DefaultSkybox => new(Image.Load<Rgba32>(AssetHelper.GetPath("Textures/cloudtop/cloudtop_ft.png")),
                Image.Load<Rgba32>(AssetHelper.GetPath("Textures/cloudtop/cloudtop_bk.png")),
                Image.Load<Rgba32>(AssetHelper.GetPath("Textures/cloudtop/cloudtop_lf.png")),
                Image.Load<Rgba32>(AssetHelper.GetPath("Textures/cloudtop/cloudtop_rt.png")),
                Image.Load<Rgba32>(AssetHelper.GetPath("Textures/cloudtop/cloudtop_up.png")),
                Image.Load<Rgba32>(AssetHelper.GetPath("Textures/cloudtop/cloudtop_dn.png")));

        private FullScreenQuad fsq;

        public SampleApp(GraphicsBackend backend = GraphicsBackend.Vulkan) : base(backend) 
        {
            Skybox skybox = DefaultSkybox;
            AddRenderable(skybox);

            AddSponzaAtriumObjects();
            _sc.Camera.Position = new Vector3(-80, 25, -4.3f);
            _sc.Camera.Yaw = -MathF.PI / 2;
            _sc.Camera.Pitch = -MathF.PI / 9;

            ShadowmapDrawer texDrawIndexeder = new (() => Window, () => _sc.NearShadowMapView);
            _resizeHandled += (w, h) => texDrawIndexeder.OnWindowResized();
            texDrawIndexeder.Position = new Vector2(10, 25);
            Scene.AddRenderable(texDrawIndexeder);

            ShadowmapDrawer texDrawIndexeder2 = new (() => Window, () => _sc.MidShadowMapView);
            _resizeHandled += (w, h) => texDrawIndexeder2.OnWindowResized();
            texDrawIndexeder2.Position = new Vector2(20 + texDrawIndexeder2.Size.X, 25);
            Scene.AddRenderable(texDrawIndexeder2);

            ShadowmapDrawer texDrawIndexeder3 = new (() => Window, () => _sc.FarShadowMapView);
            _resizeHandled += (w, h) => texDrawIndexeder3.OnWindowResized();
            texDrawIndexeder3.Position = new Vector2(30 + (texDrawIndexeder3.Size.X * 2), 25);
            Scene.AddRenderable(texDrawIndexeder3);

            ShadowmapDrawer reflectionTexDrawer = new (() => Window, () => _sc.ReflectionColorView);
            _resizeHandled += (w, h) => reflectionTexDrawer.OnWindowResized();
            reflectionTexDrawer.Position = new Vector2(40 + (reflectionTexDrawer.Size.X * 3), 25);
            Scene.AddRenderable(reflectionTexDrawer);

            ScreenDuplicator duplicator = new ();
            Scene.AddRenderable(duplicator);

            fsq = new FullScreenQuad();
            Scene.AddRenderable(fsq);

            CreateAllObjects();
        }

        private void AddSponzaAtriumObjects()
        {
            ObjParser parser = new ObjParser();
            using (FileStream objStream = File.OpenRead(AssetHelper.GetPath("Models/SponzaAtrium/sponza.obj")))
            {
                ObjFile atriumFile = parser.Parse(objStream);
                MtlFile atriumMtls;
                using (FileStream mtlStream = File.OpenRead(AssetHelper.GetPath("Models/SponzaAtrium/sponza.mtl")))
                {
                    atriumMtls = new MtlParser().Parse(mtlStream);
                }

                foreach (ObjFile.MeshGroup group in atriumFile.MeshGroups)
                {
                    Vector3 scale = new Vector3(0.1f);
                    ConstructedMeshInfo mesh = atriumFile.GetMesh(group);
                    MaterialDefinition materialDef = atriumMtls.Definitions[mesh.MaterialName];
                    ImageSharpTexture overrideTextureData = null;
                    ImageSharpTexture alphaTexture = null;
                    MaterialPropsAndBuffer materialProps = CommonMaterials.Brick;
                    if (materialDef.DiffuseTexture != null)
                    {
                        string texturePath = AssetHelper.GetPath("Models/SponzaAtrium/" + materialDef.DiffuseTexture);
                        overrideTextureData = LoadTexture(texturePath, true);
                    }
                    if (materialDef.AlphaMap != null)
                    {
                        string texturePath = AssetHelper.GetPath("Models/SponzaAtrium/" + materialDef.AlphaMap);
                        alphaTexture = LoadTexture(texturePath, false);
                    }
                    if (materialDef.Name.Contains("vase"))
                    {
                        materialProps = CommonMaterials.Vase;
                    }
                    if (group.Name == "sponza_117")
                    {
                        MirrorMesh.Plane = Plane.CreateFromVertices(
                            atriumFile.Positions[group.Faces[0].Vertex0.PositionIndex] * scale.X,
                            atriumFile.Positions[group.Faces[0].Vertex1.PositionIndex] * scale.Y,
                            atriumFile.Positions[group.Faces[0].Vertex2.PositionIndex] * scale.Z);
                        materialProps = CommonMaterials.Reflective;
                    }

                    AddTexturedMesh(
                        mesh,
                        overrideTextureData,
                        alphaTexture,
                        materialProps,
                        Vector3.Zero,
                        Quaternion.Identity,
                        scale,
                        group.Name);
                }
            }
        }

        protected override void CreateAllDeviceObjects(CommandList cl)
        {
            CommonMaterials.CreateAllDeviceObjects(GraphicsDevice, cl, _sc);
        }

        protected override void DestroyAllDeviceObjects()
        {
            CommonMaterials.DestroyAllDeviceObjects();
        }
    }
}
