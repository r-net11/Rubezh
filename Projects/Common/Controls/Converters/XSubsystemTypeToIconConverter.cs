using System;
using System.Windows.Data;
using FiresecAPI;
using FiresecAPI.Models;
using XFiresecAPI;

namespace Controls.Converters
{
	public class XSubsystemTypeToIconConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var subsystemType = (XSubsystemType)value;
			if(subsystemType == XSubsystemType.GK)
				return "/Controls;component/Images/Chip.png";
			if (subsystemType == XSubsystemType.System)
				return "/Controls;component/Images/PC.png";
			return "/Controls;component/Images/Chip.png";
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}