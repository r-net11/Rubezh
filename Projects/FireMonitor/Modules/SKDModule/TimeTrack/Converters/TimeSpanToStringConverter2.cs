using System;
using System.Windows.Data;

namespace SKDModule.Converters
{
	public class TimeSpanToStringConverter2 : IValueConverter
	{
		object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			TimeSpan timeSpan = (TimeSpan)value;
			return ((int)timeSpan.TotalHours).ToString("00") + ":" + ((int)timeSpan.Minutes).ToString("00");
		}

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}