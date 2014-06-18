using System;
using System.Windows;
using System.Windows.Data;
using ValueType = FiresecAPI.Automation.ValueType;

namespace AutomationModule.Converters
{
	class ValueTypeToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var valueType = (ValueType)parameter;
			return (ValueType)value == valueType ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}