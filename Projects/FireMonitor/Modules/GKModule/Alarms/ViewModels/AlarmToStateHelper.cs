using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XFiresecAPI;

namespace GKModule
{
	public static class AlarmToStateHelper
	{
		public static XStateType AlarmToState(XAlarmType alarmType)
		{
			switch (alarmType)
			{
				case XAlarmType.NPT:
					return XStateType.On;

				case XAlarmType.Fire1:
					return XStateType.Fire1;

				case XAlarmType.Fire2:
					return XStateType.Fire2;

				case XAlarmType.Attention:
					return XStateType.Attention;

				case XAlarmType.Failure:
					return XStateType.Failure;

				case XAlarmType.Ignore:
					return XStateType.Ignore;

				case XAlarmType.Auto:
					return XStateType.Norm;

				case XAlarmType.Service:
					return XStateType.Norm;

				case XAlarmType.Info:
					return XStateType.Test;
			}
			return XStateType.Norm;
		}
	}
}