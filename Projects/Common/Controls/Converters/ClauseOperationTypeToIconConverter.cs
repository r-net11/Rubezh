using System;
using System.Windows.Data;
using XFiresecAPI;

namespace Controls.Converters
{
	public class ClauseOperationTypeToIconConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
            try
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
            catch
            {
                return null;
            }
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
