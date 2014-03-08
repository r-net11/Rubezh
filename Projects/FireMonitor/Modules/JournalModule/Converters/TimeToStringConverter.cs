using System;
using System.Windows.Data;

namespace JournalModule.Converters
{
	public class TimeToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var dateTime = (DateTime)value;
			return dateTime.ToString();
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}