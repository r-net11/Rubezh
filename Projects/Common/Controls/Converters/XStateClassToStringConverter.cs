using StrazhAPI;
using StrazhAPI.GK;
using System;
using System.Windows.Data;

namespace Controls.Converters
{
	public class XStateClassToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return ((XStateClass)value).ToDescription();
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return (XStateClass)value;
		}
	}
}