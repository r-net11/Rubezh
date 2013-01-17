using System;
using System.Windows;
using System.Windows.Data;

namespace Controls.Converters
{
	public class BoolToGKYesNoConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (parameter != null && System.Convert.ToBoolean(parameter))
			{
				return (bool)value ? "Нет" : "Есть";
			}
			return (bool)value ? "Есть" : "Нет";
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}