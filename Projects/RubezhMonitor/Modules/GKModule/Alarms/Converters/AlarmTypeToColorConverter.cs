using System;
using System.Windows.Data;
using RubezhAPI.GK;

namespace GKModule.Converters
{
	public class AlarmTypeToColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			switch ((GKAlarmType)value)
			{
				case GKAlarmType.NPTOn:
					return "Red";

				case GKAlarmType.GuardAlarm:
					return "Red";

				case GKAlarmType.Fire1:
					return "Red";

				case GKAlarmType.Fire2:
					return "Red";

				case GKAlarmType.Attention:
					return "Orange";

				case GKAlarmType.Failure:
					return "Yellow";

				case GKAlarmType.Ignore:
					return "Wheat";

				case GKAlarmType.Turning:
					return "SkyBlue";

				case GKAlarmType.Service:
					return "SkyBlue";

				case GKAlarmType.AutoOff:
					return "Yellow";

				default:
					return "Transparent";
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}