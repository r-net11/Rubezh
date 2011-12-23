using System;
using System.Windows.Data;
using Common;
using FiresecAPI.Models;

namespace FiltersModule.Converters
{
    public class CategoryTypeToCategoryNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return EnumHelper.ToString((FiresecAPI.Models.DeviceCategoryType) value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}