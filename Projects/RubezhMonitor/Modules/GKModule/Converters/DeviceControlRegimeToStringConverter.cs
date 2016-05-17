using System;
using System.Windows.Data;
using RubezhAPI;

namespace GKModule.Converters
{
	public class DeviceControlRegimeToStringConverter : IValueConverter
	{
		object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			DeviceControlRegime deviceControlRegime = (DeviceControlRegime)value;
			return deviceControlRegime.ToDescription();
		}

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}