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
			var minutes = timeSpan.Minutes;
			var firstChar = hours < 0 || minutes < 0 ? "-" : string.Empty;
			return firstChar + Math.Abs(hours).ToString("00") + ":" + Math.Abs(minutes).ToString("00");
		}

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}