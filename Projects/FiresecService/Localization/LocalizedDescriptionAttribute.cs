using System;
using System.ComponentModel;
using System.IO;
using System.Resources;

namespace LocalizationConveters
{
    public class LocalizedDescriptionAttribute : DescriptionAttribute
    {
        public LocalizedDescriptionAttribute(Type resourceType, string resourceId)
            : base(ResourceHelper.GetResource(resourceType, resourceId))
        { }
    }
}
