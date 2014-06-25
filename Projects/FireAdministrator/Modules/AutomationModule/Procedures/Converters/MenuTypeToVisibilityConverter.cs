using System;
using System.Windows;
using System.Windows.Data;
using AutomationModule.Procedures;

namespace AutomationModule.Converters
{
	class MenuTypeToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var valueType = (MenuType)parameter;
			return (MenuType)value == valueType ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
