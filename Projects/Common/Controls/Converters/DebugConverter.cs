using System;
using System.Windows.Data;

namespace Controls.Converters
{
	public class DebugConverter : IValueConverter
	{
		public Object Convert(Object value, Type targetType, Object parameter, System.Globalization.CultureInfo culture)
		{
			return value;
		}

		public Object ConvertBack(Object value, Type targetType, Object parameter, System.Globalization.CultureInfo culture)
		{
			return value;
		}
	}
}
