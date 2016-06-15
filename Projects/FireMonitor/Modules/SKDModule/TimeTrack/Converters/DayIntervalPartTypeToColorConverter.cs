using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using StrazhAPI.SKD;

namespace SKDModule.Converters
{
	public class DayIntervalPartTypeToColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return (DayIntervalPartType) value == DayIntervalPartType.Break ? Brushes.Gray : Brushes.Black;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}