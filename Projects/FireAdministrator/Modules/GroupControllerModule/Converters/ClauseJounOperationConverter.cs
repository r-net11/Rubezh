﻿using System;
using System.Windows.Data;
using FiresecAPI;
using FiresecAPI.GK;

namespace GKModule.Converters
{
	public class ClauseJounOperationConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (value is ClauseJounOperationType)
			{
				return ((ClauseJounOperationType)value).ToDescription();
			}
			return "";
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}