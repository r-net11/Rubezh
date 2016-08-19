using System;
using System.ComponentModel;

namespace Localization.Converters
{
    public class LocalizedDescriptionAttribute : DescriptionAttribute
    {
        public LocalizedDescriptionAttribute(Type resourceType, string resourceId)
            : base(ResourceHelper.GetResource(resourceType, resourceId))
        { }
    }
}
