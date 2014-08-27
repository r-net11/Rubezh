using System;
using System.Windows.Data;
using System.Windows.Media;

namespace SKDModule.Converters
{
	public class TimeSpanToColorConverter : IValueConverter
	{
		object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			TimeSpan timeSpan = (TimeSpan)value;
			if (timeSpan != TimeSpan.Zero)
				return Brushes.Black;
			return Brushes.Gray;
		}

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}