using FiresecAPI.Automation;
using System.Windows;
using System;
using System.Windows.Data;

namespace AutomationModule.Converters
{
	class EnumToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var valueType = (VariableType)parameter;
			return (VariableType)value == valueType ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}