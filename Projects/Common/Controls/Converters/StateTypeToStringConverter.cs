using System;
using System.Windows.Data;
using FiresecAPI.Models;
using FiresecAPI;

namespace Controls.Converters
{
    public class StateTypeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return EnumsConverter.StateTypeToClassName((StateType) value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (StateType) value;
        }
    }
}