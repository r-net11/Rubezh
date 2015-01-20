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
					return "BAlarm_shield";

				case XAlarmType.Fire1:
					return "BAlarm_Main_0_Fire1";

				case XAlarmType.Fire2:
					return "BAlarm_Main_1_Fire2";

				case XAlarmType.Attention:
					return "BAlarm_main_2_Attention";

				case XAlarmType.Failure:
					return "BAlarm_main_3_Failure";

				case XAlarmType.Ignore:
					return "BAlarm_main_4_Off";

				case XAlarmType.Turning:
					return "BAlarm_main_5_Info";

				case XAlarmType.Service:
					return "BAlarm_main_6_Service";

				case XAlarmType.AutoOff:
					return "BAlarm_main_7_Auto";

				default:
					return "BAlarm_main_3_Failure";
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}