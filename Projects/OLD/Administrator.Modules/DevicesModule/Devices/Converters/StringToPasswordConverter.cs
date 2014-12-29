using System;
using System.Windows.Data;

namespace DevicesModule.Converters
{
	public class StringToPasswordConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return (string) value == null ? null : new String('*', ((string) value).Length);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value;
		}
	}
}