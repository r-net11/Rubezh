using System.Windows.Data;
using System;
namespace Controls.Converters
{
	public class RegimeToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var regime = (ushort)value;
			if (regime == 1)
				return "Включено";
			else if (regime == 2)
				return "Выключено";
			return "Неизвестно";
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}