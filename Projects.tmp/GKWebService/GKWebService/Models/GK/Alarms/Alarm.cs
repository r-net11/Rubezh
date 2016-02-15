using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RubezhAPI.GK;

namespace GKWebService.Models.GK.Alarms
{
	public class Alarm
	{
		public GKAlarmType AlarmType { get; set; }

		public GKBase GkBaseEntity { get; set; }

		public Alarm()
		{
			
		}

		public Alarm(GKAlarmType alarmType, GKBase gkBaseEntity)
		{
			AlarmType = alarmType;
			GkBaseEntity = gkBaseEntity;
		}
	}
}