using System;
using System.Windows.Data;

namespace SoundsModule.Converters
{
	internal class BoolToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return (bool) value ? "Остановить" : "Проверка";
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}