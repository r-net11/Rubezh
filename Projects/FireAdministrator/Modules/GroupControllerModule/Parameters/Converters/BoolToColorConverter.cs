﻿using System;
using System.Windows.Data;
using System.Windows.Media;
using GKModule.ViewModels;

namespace GKModule.Converters
{
	public class BoolToColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if((bool)value)
			{
				return Brushes.White;
			} else return Brushes.Red;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}