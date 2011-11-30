using System;
using System.Windows.Data;

namespace Controls.Converters
{
    public class BeeperTypeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return FiresecAPI.Models.EnumsConverter.BeeperTypeToBeeperName((FiresecAPI.Models.BeeperType) value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (FiresecAPI.Models.BeeperType) value;
        }
    }
}