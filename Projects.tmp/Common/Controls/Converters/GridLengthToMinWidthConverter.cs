using System;
using System.Windows;
using System.Windows.Data;

namespace Controls.Converters
{
	public class GridLengthToMinWidthConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var gridLength = (GridLength)value;
			return gridLength.Value == 0 && gridLength.IsAbsolute ? 0 : 200;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}