using System;
using System.Windows.Data;

namespace GKModule.Converters
{
	public class DeviceControlRegimeToIconConverter : IValueConverter
	{
		object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			DeviceControlRegime deviceControlRegime = (DeviceControlRegime)value;
			switch (deviceControlRegime)
			{
				case DeviceControlRegime.Automatic:
					return "/Controls;component/StateClassIcons/TechnologicalRegime.png";
				case DeviceControlRegime.Manual:
					return "/Controls;component/StateClassIcons/Manual.png";
				case DeviceControlRegime.Ignore:
					return "/Controls;component/StateClassIcons/Ignore.png";
			}
			return "";
		}

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}