using System;
using System.Windows;
using System.Windows.Data;

namespace Controls.Converters
{
	public class BoolToGridLengthConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return (bool)value ? new GridLength(1, GridUnitType.Star) : GridLength.Auto;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}