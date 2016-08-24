using System;
using System.ComponentModel;

namespace Localization.Converters
{
    public class LocalizedDescriptionAttribute : DescriptionAttribute
    {
        /// <summary>
        /// Заменяем стандартный DescriptionAttribute на наш, который подтягивает ресурсы
        /// </summary>
        /// <param name="resourceType">typeof(CommonResources)</param>
        /// <param name="resourceId">"CurrentWeek"</param>
        public LocalizedDescriptionAttribute(Type resourceType, string resourceId)
            : base(ResourceHelper.GetResource(resourceType, resourceId))
        { }
    }
}
