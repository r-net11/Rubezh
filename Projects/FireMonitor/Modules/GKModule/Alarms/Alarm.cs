using FiresecAPI.GK;

namespace GKModule
{
	public class Alarm
	{
		public GKAlarmType AlarmType { get; set; }
		public GKDevice Device { get; set; }
		public GKZone Zone { get; set; }
		public GKDirection Direction { get; set; }

		public Alarm(GKAlarmType alarmType, GKDevice device)
		{
			AlarmType = alarmType;
			Device = device;
		}

		public Alarm(GKAlarmType alarmType, GKZone zone)
		{
			AlarmType = alarmType;
			Zone = zone;
		}

		public Alarm(GKAlarmType alarmType, GKDirection direction)
		{
			AlarmType = alarmType;
			Direction = direction;
		}

		public Alarm Clone()
		{
			var alarm = new Alarm(AlarmType, Device);
			alarm.Zone = Zone;
			alarm.Direction = Direction;
			return alarm;
		}

		public bool IsEqualTo(Alarm alarm)
		{
			if (alarm.AlarmType != AlarmType)
				return false;
			if (alarm.Device != null && alarm.Device != Device)
				return false;
			if (alarm.Zone != null && alarm.Zone != Zone)
				return false;
			if (alarm.Direction != null && alarm.Direction != Direction)
				return false;
			return true;
		}
	}
}