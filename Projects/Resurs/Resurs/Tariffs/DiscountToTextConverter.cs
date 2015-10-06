using System;
using System.Windows;
using System.Windows.Data;

namespace Resurs.Converters
{
	public class DiscountToTextConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return (bool)value ? "Льготный тариф" : "Обычный тариф";
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}