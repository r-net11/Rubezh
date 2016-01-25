﻿using System;
using System.Windows;
using System.Windows.Data;

namespace Controls.Converters
{
	public class StringToVisibilityConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return String.IsNullOrWhiteSpace((string)value) ? Visibility.Collapsed : Visibility.Visible;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}