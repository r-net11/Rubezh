using System;
using System.Windows.Data;
using StrazhAPI;
using Infrastructure.Models;

namespace JournalModule.Converters
{
	public class JournalColumnTypeToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return ((JournalColumnType)value).ToDescription();
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value;
		}
	}
}