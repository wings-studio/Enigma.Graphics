using System;
using System.Collections.Generic;

namespace Enigma.Graphics.App
{
    public class MeshRenderTask : MeshAbstractTask<VertexColorPosition>
    {
        private Pipeline pipeline;
        private readonly List<ResourceSet> resources = new List<ResourceSet>();

        // https://github.com/dotnet/Silk.NET/blob/main/examples/CSharp/OpenGL%20Tutorials/Tutorial%201.3%20-%20Abstractions/shader.vert
        private static readonly string VertexShaderSource = @"
        //Here we specify the version of our shader.
#version 330 core
//These lines specify the location and type of our attributes,
//the attributes here are prefixed with a v as they are our inputs to the vertex shader
//this isn't strictly necessary though, but a good habit.
layout(location = 0) in vec3 vPos;
        layout(location = 1) in vec4 vColor;

        //This is how we declare a uniform, they can be used in all our shaders and share the same name.
        uniform float Blue;

//This is our output variable, notice that this is prefixed with an f as it's the input of our fragment shader.
out vec4 fColor;

        void main()
        {
            //gl_Position, is a built-in variable on all vertex shaders that will specify the position of our vertex.
            gl_Position = vec4(vPos, 1.0);
            //The rest of this code looks like plain old c (almost c#)
            vec4 color = vec4(vColor.rb / 2, Blue, vColor.a); //Swizzling and constructors in glsl.
            fColor = color;
        }";

        // https://github.com/dotnet/Silk.NET/blob/main/examples/CSharp/OpenGL%20Tutorials/Tutorial%201.3%20-%20Abstractions/shader.frag
        private static readonly string FragmentShaderSource = @"
        //Specifying the version like in our vertex shader.
#version 330 core
//The input variables, again prefixed with an f as they are the input variables of our fragment shader.
//These have to share name for now even though there is a way around this later on.
in vec4 fColor;
  
//The output of our fragment shader, this just has to be a vec3 or a vec4, containing the color information about
//each fragment or pixel of our geometry.
out vec4 FragColor;

        void main()
        {
            //Here we are setting our output variable, for which the name is not important.
            FragColor = fColor;
        }";

        public override void CreateResources(IGraphicsDevice gd, MeshScene<VertexColorPosition> scene)
        {
            IShader vs = gd.LoadShader(VertexShaderSource, ShaderStage.Vertex);
            IShader fs = gd.LoadShader(FragmentShaderSource, ShaderStage.Fragment);
            VertexElement[] vertexLayout = new[]
            {
                new VertexElement(3, "vPos", VertexElementType.Float),
                new VertexElement(4, "vColor", VertexElementType.Float)
            };
            ResourceLayout layout = new ResourceLayout(
                new ResourceElement("Blue", ResourceKind.UniformBuffer, ShaderStage.Vertex)
                );
            pipeline = gd.CreatePipeline(new[] { vs, fs }, vertexLayout, layout);
            foreach (var mesh in scene.Meshes)
                resources.Add(gd.CreateResourceSet(layout, 
                    new FloatResource((float)Math.Sin(DateTime.Now.Millisecond / 1000f * Math.PI))
                    ));
        }

        public override void Render(IGraphicsDevice gd, MeshScene<VertexColorPosition> scene, int meshIndex)
        {
            gd.SetVertexBuffer(0, scene.VertexBuffers[meshIndex]);
            gd.SetIndexBuffer(scene.IndexBuffers[meshIndex], IndexFormat.UInt);
            gd.SetPipeline(pipeline);
            gd.SetResourceSet(0, resources[meshIndex]);
            gd.DrawIndexed(scene.IndexLengths[meshIndex]);
        }
    }
}
