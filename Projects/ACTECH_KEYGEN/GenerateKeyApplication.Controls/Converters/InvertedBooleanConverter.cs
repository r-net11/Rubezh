using System;
using System.Windows.Data;

namespace GenerateKeyApplication.Controls.Converters
{
	public class InvertedBooleanConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null)
				return null;
			var v = ((bool?)value).GetValueOrDefault();
			return !v;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value == null)
				return null;
			var v = ((bool?)value).GetValueOrDefault();
			return !v;
		}
	}
}
