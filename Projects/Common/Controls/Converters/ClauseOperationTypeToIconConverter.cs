using System;
using System.Windows.Data;
using FiresecAPI.GK;

namespace Controls.Converters
{
	public class ClauseOperationTypeToIconConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			try
			{
				if (value is ClauseOperationType)
				{
					switch ((ClauseOperationType)value)
					{
						case ClauseOperationType.AllDevices:
						case ClauseOperationType.AnyDevice:
							return "/Controls;component/GKIcons/RSR2_RM_1.png";

						case ClauseOperationType.AllZones:
						case ClauseOperationType.AnyZone:
							return "/Controls;component/Images/Zone.png";

						case ClauseOperationType.AllDirections:
						case ClauseOperationType.AnyDirection:
							return "/Controls;component/Images/Blue_Direction.png";

						case ClauseOperationType.AllMPTs:
						case ClauseOperationType.AnyMPT:
							return "/Controls;component/Images/BMPT.png";

						case ClauseOperationType.AllDelays:
						case ClauseOperationType.AnyDelay:
							return "/Controls;component/Images/Delay.png";

						default:
							return "/Controls;component/Images/Zone.png";
					}
				}
			}
			catch { }
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
