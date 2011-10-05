using System;
using System.Windows.Data;
using FiresecAPI.Models;

namespace SecurityModule.Converters
{
    public class RemoteAccessTypeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return EnumsConverter.RemoteAccessTypeToString((RemoteAccessType) value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}