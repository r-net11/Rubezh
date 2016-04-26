using System;
using System.ComponentModel;
using System.Resources;

namespace Localization
{
    public class LocalizedDescriptionAttribute : DescriptionAttribute
    {
        public LocalizedDescriptionAttribute(Type resourceType, string resourceId)
            : base(GetMessageFromResource(resourceType, resourceId))
        { }

        private static string GetMessageFromResource(Type resourceType, string resourceId)
        {
            var rm = new ResourceManager(resourceType);
            if (rm.GetString(resourceId) != null)
            {
                return rm.GetString(resourceId);
            }
            return "Ooops. I couldn't find any resource.";
        }
    }
}
