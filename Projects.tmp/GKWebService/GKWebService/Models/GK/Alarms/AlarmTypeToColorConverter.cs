using System;
using RubezhAPI.GK;

namespace GKWebService.Models.GK.Alarms
{
	public class AlarmTypeToColorConverter
	{
		public string Convert(GKAlarmType value)
		{
			switch (value)
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
	}
}