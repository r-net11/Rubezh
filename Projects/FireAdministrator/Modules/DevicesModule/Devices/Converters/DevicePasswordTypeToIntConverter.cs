using System;
using System.Globalization;
using System.Windows.Data;

namespace DevicesModule.Converters
{
    public class DevicePasswordTypeToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int) value == int.Parse(parameter.ToString()) ? true : false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter;
        }
    }
}