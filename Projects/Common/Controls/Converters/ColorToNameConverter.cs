using Controls.Extentions;
using Infrastructure.Common;
using System;
using System.Windows.Data;

namespace Controls.Converters
{
	public class ColorToNameConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var color = ((System.Windows.Media.Color)value).ToRubezhColor();
			return ColorUtilities.ColorNames.ContainsKey(color) ? ColorUtilities.ColorNames[color] : null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
