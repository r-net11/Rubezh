using System;
using System.Windows.Data;
using FiresecAPI;
using Infrastructure.Models;

namespace GKModule.Converters
{
	public class JournalColumnTypeToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return ((XJournalColumnType)value).ToDescription();
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value;
		}
	}
}