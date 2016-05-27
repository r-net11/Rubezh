using System;
using System.Windows.Data;
using Controls.Converters;
using RubezhAPI.GK;

namespace GKModule.Converters
{
	public class AlarmTypeToIconPathConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			string alarmName;
			switch ((GKAlarmType) value)
			{
				case GKAlarmType.NPTOn:
					alarmName = "Alarm_MPT";
					break;

				case GKAlarmType.GuardAlarm:
					alarmName = "Shield";
					break;

				case GKAlarmType.Fire1:
					alarmName = "Alarm_Main_0_Fire1";
					break;

				case GKAlarmType.Fire2:
					alarmName = "Alarm_Main_1_Fire2";
					break;

				case GKAlarmType.Attention:
					alarmName = "Alarm_main_2_Attention";
					break;

				case GKAlarmType.Failure:
					alarmName = "Alarm_main_3_Failure";
					break;

				case GKAlarmType.Ignore:
					alarmName = "Alarm_main_4_Off";
					break;

				case GKAlarmType.Turning:
					alarmName = "Alarm_main_5_Info";
					break;

				case GKAlarmType.Service:
					alarmName = "Alarm_main_6_Service";
					break;

				case GKAlarmType.AutoOff:
					alarmName = "Alarm_main_7_Auto";
					break;

				case GKAlarmType.StopStart:
					alarmName = "Close";
					break;

				default:
					alarmName = "Alarm_main_3_Failure";
					break;
			}
			return ConverterHelper.GetResource(alarmName);
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}