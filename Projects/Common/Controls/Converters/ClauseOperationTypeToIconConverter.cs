using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using XFiresecAPI;

namespace Controls.Converters
{
	public class ClauseOperationTypeToIconConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			switch ((ClauseOperationType)value)
			{
				case ClauseOperationType.AllDevices:
				case ClauseOperationType.AnyDevice:
					return "/Controls;component/GKIcons/RSR2_RM_1.png";

				case ClauseOperationType.AllDirections:
				case ClauseOperationType.AnyDirection:
					return "/Controls;component/Images/Blue_Direction.png";

				case ClauseOperationType.AllZones:
				case ClauseOperationType.AnyZone:
					return "/Controls;component/Images/zone.png";

				default:
					return "/Controls;component/Images/zone.png";
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
