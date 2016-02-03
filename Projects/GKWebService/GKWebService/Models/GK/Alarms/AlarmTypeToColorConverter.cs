using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Windows.Data;
using RubezhAPI.GK;

namespace GKWebService.Models.GK.Alarms
{
	public class AlarmTypeToColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			switch ((GKAlarmType)value)
			{
				case GKAlarmType.NPTOn:
					return "#FF0000";

				case GKAlarmType.GuardAlarm:
					return "#FF0000";

				case GKAlarmType.Fire1:
					return "#FF0000";

				case GKAlarmType.Fire2:
					return "#FF0000";

				case GKAlarmType.Attention:
					return "#FFA500";

				case GKAlarmType.Failure:
					return "#FFFF00"; 

				case GKAlarmType.Ignore:
					return "#F5DEB3";

				case GKAlarmType.Turning:
					return "#87CEEB";

				case GKAlarmType.Service:
					return "#87CEEB";

				case GKAlarmType.AutoOff:
					return "#FFFF00";

				default:
					return "#FFFFFF";
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}