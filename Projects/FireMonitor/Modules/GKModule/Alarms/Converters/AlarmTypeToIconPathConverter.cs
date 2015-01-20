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
					return "/Controls;component/Images/Alarm_MPT.png";

				case GKAlarmType.Fire1:
					return "/Controls;component/Images/Alarm_Main_0_Fire1.png";

				case GKAlarmType.Fire2:
					return "/Controls;component/Images/Alarm_Main_1_Fire2.png";

				case GKAlarmType.Attention:
					return "/Controls;component/Images/Alarm_main_2_Attention.png";

				case GKAlarmType.Failure:
					return "/Controls;component/Images/Alarm_main_3_Failure.png";

				case GKAlarmType.Ignore:
					return "/Controls;component/Images/Alarm_main_4_Off.png";

				case GKAlarmType.Turning:
					return "/Controls;component/Images/Alarm_main_5_Info.png";

				case GKAlarmType.Service:
					return "/Controls;component/Images/Alarm_main_6_Service.png";

				case GKAlarmType.AutoOff:
					return "/Controls;component/Images/Alarm_main_7_Auto.png";

				default:
					return "/Controls;component/Images/Alarm_main_3_Failure.png";
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}