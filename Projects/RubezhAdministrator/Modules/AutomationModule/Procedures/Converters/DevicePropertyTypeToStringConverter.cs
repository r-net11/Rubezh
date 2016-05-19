using System;
using System.Windows.Data;
using FiresecAPI;
using FiresecAPI.Automation;

namespace AutomationModule.Converters
{
	class DevicePropertyTypeToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (!(value is DevicePropertyType))
				return "";
			return ((DevicePropertyType)value).ToDescription();
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}