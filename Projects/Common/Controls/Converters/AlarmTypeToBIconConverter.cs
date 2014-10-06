using System;
using System.Windows.Data;
using FiresecAPI.GK;

namespace Controls.Converters
{
	public class AlarmTypeToBIconConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			switch ((GKAlarmType)value)
			{
				case GKAlarmType.NPTOn:
					return "/Controls;component/StateClassIcons/NPTOn.png";

				case GKAlarmType.Fire1:
					return "/Controls;component/StateClassIcons/Fire1.png";

				case GKAlarmType.Fire2:
					return "/Controls;component/StateClassIcons/Fire2.png";

				case GKAlarmType.Attention:
					return "/Controls;component/StateClassIcons/Attention.png";

				case GKAlarmType.Failure:
					return "/Controls;component/StateClassIcons/Failure.png";

				case GKAlarmType.Ignore:
					return "/Controls;component/StateClassIcons/Ignore.png";

				case GKAlarmType.Turning:
					return "/Controls;component/StateClassIcons/On.png";

				case GKAlarmType.Service:
					return "/Controls;component/StateClassIcons/Service.png";

				case GKAlarmType.AutoOff:
					return "/Controls;component/StateClassIcons/AutoOff.png";

				default:
					return "/Controls;component/Images/blank.png";
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}
