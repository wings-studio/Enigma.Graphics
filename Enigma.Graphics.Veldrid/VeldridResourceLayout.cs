using System;
using System.Collections.Generic;
using Veldrid;

namespace Enigma.Graphics.Veldrid
{
    public class VeldridResourceLayout : ResourceLayout
    {
        public readonly global::Veldrid.ResourceLayout VdLayout;

        public VeldridResourceLayout(ResourceFactory factory, params ResourceElement[] elements) : base(elements)
        {
            List<ResourceLayoutElementDescription> rleds = new List<ResourceLayoutElementDescription>();
            foreach (ResourceElement re in elements)
            {
                rleds.Add(new ResourceLayoutElementDescription(re.Name,
                    VeldridUtil.FromEnigmaResource(re.Kind),
                    VeldridUtil.FromEnigmaShader(re.Stage)));
            }
            VdLayout = factory.CreateResourceLayout(new ResourceLayoutDescription(rleds.ToArray()));
        }
    }
}
