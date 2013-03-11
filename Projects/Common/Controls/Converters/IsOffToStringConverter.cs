using System;
using System.Windows.Data;

namespace Controls.Converters
{
	public class IsOffToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return (bool)value ? "Снять отключение" : "Отключить";
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}