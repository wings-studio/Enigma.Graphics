using System;
using System.Collections.Generic;
using System.Numerics;
using Veldrid.Utilities;

namespace Enigma.Graphics.Objects
{
    public class ObjectScene : Scene
    {
        public Camera Camera { get; set; }

        private BoundingFrustum CameraFrustum => new BoundingFrustum(Camera.ViewMatrix * Camera.ProjectionMatrix);
        private readonly Octree<RenderObject> renderObjects
            = new Octree<RenderObject>(new BoundingBox(Vector3.One * -50, Vector3.One * 50), 2);

        public ObjectScene() : base()
        {
            Camera = new Camera(Window.Width, Window.Height);
            Window.OnResized += () => Camera.WindowResized(Window.Width, Window.Height);
        }

        public override void Add(IRenderable renderable)
        {
            if (renderable is RenderObject ro)
            {
                ro.CreateDeviceObjects(GraphicsDevice, cl);
                renderObjects.AddItem(ro.BoundingBox, ro);
            }
            else
            {
                base.Add(renderable);
            }
        }

        public void RenderVisibleObjects()
        {
            List<RenderObject> ro = new();
            renderObjects.GetContainedObjects(CameraFrustum, ro);
            foreach (RenderObject r in ro)
            {
                r.Render(cl);
            }
        }

        public override void Draw(float deltaSeconds)
        {
            BeginDraw(cl);
            RenderVisibleObjects();
            Render(cl);
            EndDraw(cl);
            UpdateResources();
        }
    }
}
