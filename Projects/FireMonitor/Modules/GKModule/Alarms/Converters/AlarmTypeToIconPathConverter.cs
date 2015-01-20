using System;
using System.Windows.Data;
using FiresecAPI.GK;

namespace GKModule.Converters
{
	public class AlarmTypeToIconPathConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			switch ((GKAlarmType) value)
			{
				case GKAlarmType.NPTOn:
					return "Alarm_MPT";

				case GKAlarmType.Fire1:
					return "Alarm_Main_0_Fire1";

				case GKAlarmType.Fire2:
					return "Alarm_Main_1_Fire2";

				case GKAlarmType.Attention:
					return "Alarm_main_2_Attention";

				case GKAlarmType.Failure:
					return "Alarm_main_3_Failure";

				case GKAlarmType.Ignore:
					return "Alarm_main_4_Off";

				case GKAlarmType.Turning:
					return "Alarm_main_5_Info";

				case GKAlarmType.Service:
					return "Alarm_main_6_Service";

				case GKAlarmType.AutoOff:
					return "Alarm_main_7_Auto";

				default:
					return "Alarm_main_3_Failure";
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}