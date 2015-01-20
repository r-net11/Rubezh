using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Controls.Converters
{
	public class StringToResourceConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			if (value == null)
				return DependencyProperty.UnsetValue;
			if (value is string)
				try
				{
					return Application.Current.FindResource(value as string);
				}
				catch
				{
					return DependencyProperty.UnsetValue;
				}
			return DependencyProperty.UnsetValue;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
