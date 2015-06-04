using System;
using System.Windows.Data;
using FiresecAPI.GK;

namespace Controls.Converters
{
	public class GKGuardZoneDeviceActionTypeToIconConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			if (!(value is GKGuardZoneDeviceActionType))
				return null;
			var actionType = (GKGuardZoneDeviceActionType)value;
			switch(actionType)
			{
				case GKGuardZoneDeviceActionType.SetGuard:
					return "/Controls;component/StateClassIcons/On.png";

				case GKGuardZoneDeviceActionType.ResetGuard:
					return "/Controls;component/StateClassIcons/Off.png";

				case GKGuardZoneDeviceActionType.ChangeGuard:
					return "/Controls;component/StateClassIcons/Test.png";

				case GKGuardZoneDeviceActionType.SetAlarm:
					return "/Controls;component/StateClassIcons/Attention.png";
			}
			return null;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}