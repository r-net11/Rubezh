using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using FiresecAPI.Models;

namespace JournalModule.Converters
{
    public class CategoriesToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var categories = value as List<DeviceCategoryType>;
            StringBuilder result = new StringBuilder();

            if (categories != null)
            {
                for (var i = 0; i < categories.Count; ++i)
                {
                    if (i > 0)
                    {
                        result.Append(" или ");
                    }
                    result.Append(EnumsConverter.CategoryTypeToCategoryName(categories[i]));
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