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
							return "Zone";

						case ClauseOperationType.AllDirections:
						case ClauseOperationType.AnyDirection:
							return "Blue_Direction";

						case ClauseOperationType.AllMPTs:
						case ClauseOperationType.AnyMPT:
							return "BMPT";

						case ClauseOperationType.AllDelays:
						case ClauseOperationType.AnyDelay:
							return "Delay";

						default:
							return "Zone";
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
