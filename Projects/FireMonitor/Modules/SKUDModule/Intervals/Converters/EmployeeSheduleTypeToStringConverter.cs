using System;
using System.Windows.Data;
using FiresecAPI;

namespace SKDModule.Converters
{
	public class EmployeeSheduleTypeToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return ((FiresecAPI.EmployeeTimeIntervals.ScheduleSchemeType)value).ToDescription();
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value;
		}
	}
}