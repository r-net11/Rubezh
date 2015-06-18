using FiresecAPI.GK;

namespace GKModule
{
	public class Alarm
	{
		public GKAlarmType AlarmType { get; set; }
		public GKDevice Device { get; set; }
		public GKDoor Door { get; set; }

		public Alarm(GKAlarmType alarmType, GKDevice device)
		{
			AlarmType = alarmType;
			Device = device;
		}

		public Alarm(GKAlarmType alarmType)
		{
			AlarmType = alarmType;
		}

		public Alarm(GKAlarmType alarmType, GKDoor door)
		{
			AlarmType = alarmType;
			Door = door;
		}

		public Alarm Clone()
		{
			var alarm = new Alarm(AlarmType, Device);
			return alarm;
		}

		public bool IsEqualTo(Alarm alarm)
		{
			if (alarm.AlarmType != AlarmType)
				return false;
			if (alarm.Device != null && alarm.Device != Device)
				return false;
			return true;
		}
	}
}