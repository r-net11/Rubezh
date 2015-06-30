using System;
using System.Windows.Data;

namespace SKDModule.Converters
{
	public class TimeSpanToStringConverter : IValueConverter
	{
		object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			TimeSpan timeSpan = (TimeSpan)value;
			var hours = (int)timeSpan.TotalHours;
			var minutes = (int)timeSpan.Minutes;
			if (minutes < 0)
				minutes *= -1;
			return hours.ToString("00") + ":" + minutes.ToString("00");
		}

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}