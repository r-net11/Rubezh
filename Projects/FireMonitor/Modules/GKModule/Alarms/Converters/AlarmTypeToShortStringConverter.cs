using System;
using System.Windows.Data;
using RubezhAPI.GK;

namespace GKModule.Converters
{
	public class AlarmTypeToShortStringConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			switch ((GKAlarmType) value)
			{
				case GKAlarmType.NPTOn:
					return "НП";

				case GKAlarmType.GuardAlarm:
					return "ОТ";

				case GKAlarmType.Fire1:
					return "П1";

				case GKAlarmType.Fire2:
					return "П2";

				case GKAlarmType.Attention:
					return "В";

				case GKAlarmType.Failure:
					return "Н";

				case GKAlarmType.Ignore:
					return "О";

				case GKAlarmType.Turning:
					return "ВК";

				case GKAlarmType.Service:
					return "ТО";

				case GKAlarmType.AutoOff:
					return "АО";

				default:
					return "";
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}