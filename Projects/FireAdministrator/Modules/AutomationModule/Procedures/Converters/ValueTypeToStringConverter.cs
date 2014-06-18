using System;
using System.Windows.Data;
using FiresecAPI;
using ValueType = FiresecAPI.Automation.ValueType;

namespace AutomationModule.Converters
{
	class ValueTypeToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (!(value is ValueType))
				return "";
			return ((ValueType)value).ToDescription();
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}