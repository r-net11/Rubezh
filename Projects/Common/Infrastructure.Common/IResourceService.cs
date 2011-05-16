using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Infrastructure.Common
{
    public interface IResourceService
    {
        void AddResource(ResourceDescription description);
    }
}
