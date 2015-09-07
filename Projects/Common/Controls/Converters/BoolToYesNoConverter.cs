using System;
using System.Windows.Data;

namespace Controls.Converters
{
	public class BoolToYesNoConverter : IValueConverter
	{
		object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			bool boolValue = (bool)value;
			if (boolValue)
				return "Да";
			return "Нет";
		}

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}