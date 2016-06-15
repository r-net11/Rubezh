using System;
using System.Globalization;
using System.Windows.Data;
using StrazhAPI.SKD;

namespace SKDModule.Converters
{
	public class DayIntervalPartTransitionTypeToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (DayIntervalPartTransitionType) value == DayIntervalPartTransitionType.Night ? "X" : null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}