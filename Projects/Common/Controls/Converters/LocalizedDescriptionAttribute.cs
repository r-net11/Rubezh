using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Resources;
using System.Text;

namespace Controls.Converters
{
    public class LocalizedDescriptionAttribute:DescriptionAttribute
    {
        public LocalizedDescriptionAttribute(Type resourceType, string resourceId)
            : base(GetMessageFromResource(resourceType, resourceId))
        { }

        private static string GetMessageFromResource(Type resourceType, string resourceId)
        {
            var rm = new ResourceManager(resourceType);

            return rm.GetString(resourceId);
        }
    }
}
