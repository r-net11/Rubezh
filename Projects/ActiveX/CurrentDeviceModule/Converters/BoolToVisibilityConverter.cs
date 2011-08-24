using System;
using System.Windows;
using System.Windows.Data;

namespace CurrentDeviceModule.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool isRevert = false;
            if (parameter != null)
            {
                isRevert = System.Convert.ToBoolean(parameter);
            }

            bool boolValue = (bool)value;

            if (isRevert)
            {
                return boolValue ? Visibility.Collapsed : Visibility.Visible;
            }
            else
            {
                return boolValue ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
