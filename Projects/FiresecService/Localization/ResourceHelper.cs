using System;
using System.Resources;

namespace Localization
{
    public class ResourceHelper
    {
        public static string GetResource(Type resourceType, string resourceId)
        {
            var rm = new ResourceManager(resourceType);
            if (rm.GetString(resourceId) != null)
            {
                return rm.GetString(resourceId);
            }
            else
            {
                //TODO: Add logger
                return "Resource is not found.";
            }
        }
    }
}
