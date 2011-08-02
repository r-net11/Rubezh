using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using FiresecAPI.Models;

namespace FiltersModule.Converters
{
    public class CategoriesToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var categories = value as List<DeviceCategory>;
            StringBuilder result = new StringBuilder();

            if (categories != null)
            {
                for (var i = 0; i < categories.Count; ++i)
                {
                    if (i > 0)
                    {
                        result.Append(" или ");
                    }
                    result.Append(categories[i].DeviceCategoryName);
                }
            }

            return result.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
