using System;
using System.Windows;
using System.Windows.Data;

namespace Controls.Converters
{
	public class BoolToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var visible = (bool?)value;
			if (visible.HasValue)
				return (visible.Value ? Visibility.Visible : Visibility.Collapsed);
			return Visibility.Hidden;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}