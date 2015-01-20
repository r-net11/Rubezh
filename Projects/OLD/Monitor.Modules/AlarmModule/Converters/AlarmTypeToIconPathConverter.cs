using System;
using System.Windows.Data;
using FiresecAPI.Models;

namespace AlarmModule.Converters
{
	public class AlarmTypeToIconPathConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			switch ((AlarmType) value)
			{
				case AlarmType.Guard:
					return "Alarm_shield_2";

				case AlarmType.Fire:
					return "Alarm_Main_0_Fire1";

				case AlarmType.Attention:
					return "Alarm_main_2_Attention";

				case AlarmType.Failure:
					return "Alarm_main_3_Failure";

				case AlarmType.Off:
					return "Alarm_main_4_Off";

				case AlarmType.Info:
					return "Alarm_main_5_Info_2";

				case AlarmType.Service:
					return "Alarm_main_6_Service";

				case AlarmType.Auto:
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