using Common;
using System;
using System.Windows.Data;

namespace Controls.Converters
{
	public class ColorToNameConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			Color color = (Color)value;
			return ColorUtilities.ColorNames.ContainsKey(color) ? ColorUtilities.ColorNames[color] : null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
