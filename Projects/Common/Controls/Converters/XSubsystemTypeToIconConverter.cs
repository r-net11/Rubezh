using System;
using System.Windows.Data;
using FiresecAPI.GK;

namespace Controls.Converters
{
	public class XSubsystemTypeToIconConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			var subsystemType = (GKSubsystemType)value;
			if(subsystemType == GKSubsystemType.GK)
				return "/Controls;component/Images/Chip.png";
			if (subsystemType == GKSubsystemType.System)
				return "/Controls;component/Images/PC.png";
			return "/Controls;component/Images/Chip.png";
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}