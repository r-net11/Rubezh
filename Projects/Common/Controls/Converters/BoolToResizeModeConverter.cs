using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Controls.Converters
{
	class BoolToResizeModeConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (bool)value ? ResizeMode.CanResize : ResizeMode.NoResize;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
