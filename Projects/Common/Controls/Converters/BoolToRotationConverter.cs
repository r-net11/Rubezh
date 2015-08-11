using System;
using System.Windows.Data;

namespace Controls.Converters
{
	public class BoolToRotationConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var val = parameter != null && System.Convert.ToBoolean(parameter) ? !(bool)value : (bool)value;
			return val ? "0" : "180";
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}