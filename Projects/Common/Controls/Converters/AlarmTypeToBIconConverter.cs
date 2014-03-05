using System;
using System.Windows.Data;
using XFiresecAPI;

namespace Controls.Converters
{
	public class AlarmTypeToBIconConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			switch ((XAlarmType)value)
			{
				case XAlarmType.NPTOn:
					return "/Controls;component/StateClassIcons/NPTOn.png";

				case XAlarmType.Fire1:
					return "/Controls;component/StateClassIcons/Fire1.png";

				case XAlarmType.Fire2:
					return "/Controls;component/StateClassIcons/Fire2.png";

				case XAlarmType.Attention:
					return "/Controls;component/StateClassIcons/Attention.png";

				case XAlarmType.Failure:
					return "/Controls;component/StateClassIcons/Failure.png";

				case XAlarmType.Ignore:
					return "/Controls;component/StateClassIcons/Ignore.png";

				case XAlarmType.Turning:
					return "/Controls;component/StateClassIcons/On.png";

				case XAlarmType.Service:
					return "/Controls;component/StateClassIcons/Service.png";

				case XAlarmType.AutoOff:
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
