using System;
using System.Windows.Data;
using Localization.Common.Controls;

namespace Controls.Converters
{
	public class DateToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (!(value is int))
				return "";
			if ((int)value == -1)
				return CommonResources.Any;
			return value;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value;
		}
	}
}