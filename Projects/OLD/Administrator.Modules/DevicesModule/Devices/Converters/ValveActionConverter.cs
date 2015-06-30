using System;
using System.Windows.Data;

namespace DevicesModule.Converters
{
	public class ValveActionConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			switch ((string) value)
			{
				case "0":
					return "Закрытие";

				case "1":
					return "Открытие";

				default:
					return "Закрытие";
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}