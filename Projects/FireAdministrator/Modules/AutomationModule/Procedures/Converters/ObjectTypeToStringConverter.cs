using System;
using System.Windows.Data;
using FiresecAPI;
using FiresecAPI.Models;

namespace AutomationModule.Converters
{
	public class ObjectTypeToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return ((ProcedureObjectType)value).ToDescription();
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}