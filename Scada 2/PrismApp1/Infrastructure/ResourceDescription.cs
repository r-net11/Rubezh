using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Infrastructure
{
    public class ResourceDescription
    {
        public ResourceDescription(Assembly callerAssembly, string name)
        {
            Source = ResourceHelper.ComposeResourceUri(callerAssembly, name);
        }

        public Uri Source { get; private set; }
    }
}
