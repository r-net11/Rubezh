using RubezhAPI.GK;

namespace GKModule
{
	public class Alarm
	{
		public GKAlarmType AlarmType { get; set; }
		public GKBase GkBaseEntity { get; set; }

		public Alarm(GKAlarmType alarmType, GKBase gkBaseEntity)
		{
			AlarmType = alarmType;
			GkBaseEntity = gkBaseEntity;
		}

		public Alarm Clone()
		{
			var alarm = new Alarm(AlarmType, GkBaseEntity);
			return alarm;
		}

		public bool IsEqualTo(Alarm alarm)
		{
			if (alarm.AlarmType != AlarmType)
				return false;
			if (alarm.GkBaseEntity != GkBaseEntity)
				return false;
			return true;
		}
	}
}