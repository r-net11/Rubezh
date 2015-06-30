using System;
using System.Windows;
using System.Windows.Data;

namespace CurrentDeviceModule.Converters
{
    public class StateTypeToStringConverter2 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return FiresecAPI.Models.EnumsConverter.StateTypeToClassName((FiresecAPI.Models.StateType) value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return (FiresecAPI.Models.StateType) value;
            //throw new NotImplementedException();
        }
    }
}