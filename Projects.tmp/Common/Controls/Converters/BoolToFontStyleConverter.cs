using System;
using System.Windows;
using System.Windows.Data;

namespace Controls.Converters
{
	public class BoolToFontStyleConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return (bool)value ? FontStyles.Italic : FontStyles.Normal;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}