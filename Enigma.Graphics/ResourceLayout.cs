using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enigma.Graphics
{
    public class ResourceLayout
    {
        public readonly ResourceElement[] elements;

        public ResourceLayout(params ResourceElement[] elements)
        {
            this.elements = elements;
        }
    }
}
