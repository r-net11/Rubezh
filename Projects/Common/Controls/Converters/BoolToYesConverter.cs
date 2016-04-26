using System;
using System.Windows.Data;

namespace Controls.Converters
{
	public class BoolToYesConverter : IValueConverter
	{
		object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			bool boolValue = (bool)value;
			if (boolValue)
				return Resources.Language.BoolToYesConverter.Yes;
			return "";
		}

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}