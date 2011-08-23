using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;
using FiresecAPI.Models;
using Common;

namespace JournalModule.Converters
{
    public class CategoriesToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var categories = value as List<DeviceCategoryType>;

            if (categories.IsNotNullOrEmpty())
            {
                var delimString = " или ";
                var result = new StringBuilder();

                foreach (var category in categories)
                {
                    result.Append(EnumsConverter.CategoryTypeToCategoryName(category));
                    result.Append(delimString);
                }

                return result.ToString().Remove(result.Length - delimString.Length);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}