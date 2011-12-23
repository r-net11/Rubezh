using System;
using System.Windows.Data;
using Common;
using FiresecAPI.Models;

namespace Controls.Converters
{
    public class BeeperTypeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return EnumHelper.ToString((FiresecAPI.Models.BeeperType)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (FiresecAPI.Models.BeeperType) value;
        }
    }
}