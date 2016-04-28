using System;

namespace Localization
{
    public class LocalizedDeviceTypeLabel:Attribute
    {
        public LocalizedDeviceTypeLabel(Type resourceType, string resourceId = (string) null)
        {
            Type = ResourceHelper.GetResource(resourceType, resourceId);
            Label = string.Format("SR-{0}", resourceId);
        }

        public string Type { get; set; }

        public string Label { get; set; }
    }
}
