using System;
using System.Windows.Data;
using XFiresecAPI;

namespace GKModule.Converters
{
	public class AlarmTypeToBIconConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			switch ((XAlarmType)value)
			{
				case XAlarmType.NPTOn:
					return "/Controls;component/Images/BAlarm_shield.png";

				case XAlarmType.Fire1:
					return "/Controls;component/Images/BAlarm_Main_0_Fire1.png";

				case XAlarmType.Fire2:
					return "/Controls;component/Images/BAlarm_Main_1_Fire2.png";

				case XAlarmType.Attention:
					return "/Controls;component/Images/BAlarm_main_2_Attention.png";

				case XAlarmType.Failure:
					return "/Controls;component/Images/BAlarm_main_3_Failure.png";

				case XAlarmType.Ignore:
					return "/Controls;component/Images/BAlarm_main_4_Off.png";

				case XAlarmType.Turning:
					return "/Controls;component/Images/BAlarm_main_5_Info.png";

				case XAlarmType.Service:
					return "/Controls;component/Images/BAlarm_main_6_Service.png";

				case XAlarmType.AutoOff:
					return "/Controls;component/Images/BAlarm_main_7_Auto.png";

				default:
					return "/Controls;component/Images/BAlarm_main_3_Failure.png";
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}