﻿using System;
using System.Windows.Data;
using Entities.DeviceOriented;

namespace VideoModule.Converters
{
	public class ChannelToStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var channel = value as Channel;
			if (channel != null)
				return channel.Name;
			return "<нет>";
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value;
		}
	}
}