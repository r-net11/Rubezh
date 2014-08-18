using System;
using System.Windows.Data;

namespace SKDModule.Converters
{
	public class TimeSpanToStringConverter : IValueConverter
	{
		object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			TimeSpan timeSpan = (TimeSpan)value;
			if (timeSpan.TotalHours > 0)
				return ((int)timeSpan.TotalHours).ToString() + "ч " + ((int)timeSpan.Minutes).ToString() + "мин";
			else if (timeSpan.Minutes > 0)
				return ((int)timeSpan.Minutes).ToString() + "мин";
			return null;
		}

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}