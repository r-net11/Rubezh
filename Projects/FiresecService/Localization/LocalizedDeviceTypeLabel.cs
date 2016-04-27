using System;
using System.Reflection;
using System.Resources;

namespace Localization
{
    public class LocalizedDeviceTypeLabel:Attribute
    {
        public LocalizedDeviceTypeLabel(Type resourceType, string resourceId = (string) null)
        {
            Type = ResourceHelper.GetResource(resourceType, resourceId);
            Label = string.Format("SR-{0}",resourceId);
        }

        public string Type { get; set; }
        public string Label { get; set; }
    }

    class ResourceHelper
    {
        public static string GetResource(Type resourceType, string resourceId)
        {
            var rm = new ResourceManager(resourceType);
            if (rm.GetString(resourceId) != null)
            {
                return rm.GetString(resourceId);
            }
            return null;
        }
    }
}
