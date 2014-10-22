using System;
using System.Windows.Data;

namespace Controls.Converters
{
	public class NullableTimeToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var dateTime = (DateTime?)value;
			if (dateTime.HasValue)
				return dateTime.Value.ToString();
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}