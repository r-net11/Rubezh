using System;
using System.Windows;
using System.Windows.Data;

namespace Controls.Converters
{
	class EnumToBoolConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (!value.GetType().IsEnum)
				return false;
			return Equals(value, parameter);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}