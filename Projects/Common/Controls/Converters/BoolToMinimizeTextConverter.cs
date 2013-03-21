using System;
using System.Windows;
using System.Windows.Data;

namespace Controls.Converters
{
	public class BoolToMinimizeTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
			var val = parameter != null && System.Convert.ToBoolean(parameter) ? !(bool)value : (bool)value;
            return val ? "<<" : ">>";
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}