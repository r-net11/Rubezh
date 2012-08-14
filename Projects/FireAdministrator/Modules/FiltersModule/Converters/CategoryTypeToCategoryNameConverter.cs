using System;
using System.Windows.Data;
using FiresecAPI;
using FiresecAPI.Models;

namespace FiltersModule.Converters
{
    public class CategoryTypeToCategoryNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((FiresecAPI.Models.DeviceCategoryType) value).ToDescription();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}