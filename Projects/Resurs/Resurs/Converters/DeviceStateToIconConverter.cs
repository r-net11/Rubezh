using ResursAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;

namespace Resurs.Converters
{
	public class DeviceStateToIconConverter : IValueConverter
	{
		object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			DeviceState deviceState = (DeviceState)value;
			switch (deviceState)
			{
				case DeviceState.ConnectionLost:
					return "/Controls;component/StateClassIcons/ConnectionLost.png";
				case DeviceState.Disabled:
					return "/Controls;component/StateClassIcons/Unknown.png";
				case DeviceState.Error:
					return "/Controls;component/StateClassIcons/Manual.png";
				case DeviceState.Norm:
					return "/Controls;component/Images/Blank.png";
			}
			return "";
		}

		object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
