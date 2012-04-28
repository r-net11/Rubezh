using System;
using System.Windows.Data;

namespace Controls.Converters
{
	public class DateTimeToStringConverter : IValueConverter
	{
		public string Format { get; set; }
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return ((DateTime)value).ToString(Format);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}