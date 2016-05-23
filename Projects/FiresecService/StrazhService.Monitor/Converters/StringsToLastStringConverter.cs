using System;
using System.Globalization;
using System.Linq;
using System.Windows.Data;

namespace StrazhService.Monitor.Converters
{
	public class StringsToLastStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
				return null;
			var strings = (string) value;
			var splitedStrings = strings.Split(new[] {"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
			return splitedStrings.Any() ? splitedStrings.Last() : null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}