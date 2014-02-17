using System;
using System.Windows.Data;
using FiresecAPI;
using FiresecAPI.Models;

namespace SKDModule.Converters
{
	public class EmployeeHolidayTypeToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return ((EmployeeHolidayType)value).ToDescription();
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value;
		}
	}
}