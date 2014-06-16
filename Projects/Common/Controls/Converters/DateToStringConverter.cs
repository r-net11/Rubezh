using System;
using System.Windows.Data;

namespace Controls.Converters
{
	public class DateToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			try
			{
				if ((int)value == -1)
					return "Любой";
				return value;
			}
			catch (Exception)
			{
				return "";
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value;
		}
	}
}