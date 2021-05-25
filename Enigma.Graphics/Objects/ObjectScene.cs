﻿using System;
using System.Collections.Generic;
using System.Numerics;
using Veldrid.Utilities;

namespace Enigma.Graphics.Objects
{
    public class ObjectScene : Scene
    {
        public Camera Camera { get; set; }

        private readonly Octree<RenderObject> renderObjects
            = new Octree<RenderObject>(new BoundingBox(Vector3.One * -50, Vector3.One * 50), 2);
        private BoundingFrustum cameraFrustum => new BoundingFrustum(Camera.ViewMatrix * Camera.ProjectionMatrix);

        public ObjectScene() : base()
        {
            Camera = new Camera(Window.Width, Window.Height);
            Window.OnResized += () => Camera.WindowResized(Window.Width, Window.Height);
        }

        public override void Add(IRenderable renderable)
        {
            if (renderable is RenderObject ro)
            {
                ro.GraphicsDevice = GraphicsDevice;
                renderObjects.AddItem(ro.BoundingBox, ro);
            }
            else
            {
                base.Add(renderable);
            }
        }

        public override void Draw(float deltaSeconds)
        {
            BeginDraw();
            List<RenderObject> ro = new ();
            renderObjects.GetContainedObjects(cameraFrustum, ro);
            foreach (RenderObject r in ro)
            {
                r.CommandList = cl;
                r.Render();
            }
            Render();
            EndDraw();
            UpdateResources();
        }
    }
}
