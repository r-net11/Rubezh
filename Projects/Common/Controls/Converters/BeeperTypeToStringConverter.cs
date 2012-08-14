using System;
using System.Windows.Data;
using FiresecAPI;

namespace Controls.Converters
{
    public class BeeperTypeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((FiresecAPI.Models.BeeperType) value).ToDescription();
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (FiresecAPI.Models.BeeperType) value;
        }
    }
}