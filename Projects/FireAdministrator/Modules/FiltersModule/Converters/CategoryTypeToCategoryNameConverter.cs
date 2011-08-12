using System;
using System.Windows.Data;

namespace FiltersModule.Converters
{
    public class CategoryTypeToCategoryNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return FiresecAPI.Models.EnumsConverter.CategoryTypeToCategoryName(
                (FiresecAPI.Models.DeviceCategoryType) value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}