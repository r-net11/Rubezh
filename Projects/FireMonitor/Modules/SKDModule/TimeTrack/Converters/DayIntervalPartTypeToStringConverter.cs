using System;
using System.Globalization;
using System.Windows.Data;
using StrazhAPI.SKD;

namespace SKDModule.Converters
{
	public class DayIntervalPartTypeToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (DayIntervalPartType) value == DayIntervalPartType.Break ? "X" : null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}