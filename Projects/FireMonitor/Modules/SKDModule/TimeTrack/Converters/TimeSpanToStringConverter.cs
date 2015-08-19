using System;
using System.Windows.Data;

namespace SKDModule.Converters
{
	public class TimeSpanToStringConverter : IValueConverter
	{
		object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			TimeSpan timeSpan = (TimeSpan)value;
			string minus = "";
			var hours = (int)timeSpan.TotalHours;
			var minutes = (int)timeSpan.Minutes;
			if (hours < 0 || (hours == 0 && minutes < 0))
			{
				hours *= -1;
				minutes *= -1;
				minus = "-";
			}
			return minus + hours.ToString("00") + ":" + minutes.ToString("00");
		}

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}