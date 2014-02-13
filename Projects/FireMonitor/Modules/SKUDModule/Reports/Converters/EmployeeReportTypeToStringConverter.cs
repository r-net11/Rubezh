using System;
using System.Windows.Data;
using FiresecAPI;
using FiresecAPI.Models;
using SKDModule.Models;

namespace SKDModule.Converters
{
	public class EmployeeReportTypeToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return ((EmployeeReportType)value).ToDescription();
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value;
		}
	}
}