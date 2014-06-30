using System;
using System.Windows;
using System.Windows.Data;
using FiresecAPI.Automation;

namespace AutomationModule.Converters
{
	class ObjectTypeToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var valueType = (ObjectType)parameter;
			return (ObjectType)value == valueType ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}