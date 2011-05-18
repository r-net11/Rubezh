using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DeviceEditor.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public bool Not { get; set; }

        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var boolValue = (bool) value;
            return boolValue ^ Not ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}