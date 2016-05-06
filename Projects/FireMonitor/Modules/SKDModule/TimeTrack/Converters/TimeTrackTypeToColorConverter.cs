using System;
using System.Windows.Data;
using System.Windows.Media;
using StrazhAPI.SKD;

namespace SKDModule.Converters
{
	public class TimeTrackTypeToColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var color = value as Color?;
			if (color == null) return null;

			return new SolidColorBrush(color.Value);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value;
		}
	}
}