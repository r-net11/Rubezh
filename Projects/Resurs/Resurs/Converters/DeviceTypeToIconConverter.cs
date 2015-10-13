using ResursAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Resurs.Converters
{
	public class DeviceTypeToIconConverter : IValueConverter
	{
		object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			DeviceType deviceType = (DeviceType)value;
			switch (deviceType)
			{
				case DeviceType.Counter:
					return "/Controls;component/SKDIcons/Controller.png";
				case DeviceType.Network:
					return "/Controls;component/SKDIcons/LockControl.png";
				case DeviceType.System:
					return "/Controls;component/SKDIcons/System.png";
			}
			return "";
		}

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
