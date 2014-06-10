using System;
using System.Windows.Data;
using FiresecAPI;

namespace Controls.Converters
{
	public class DateToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if ((int)value != -1)
				return value;
			return "Любой";
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value;
		}
	}
}