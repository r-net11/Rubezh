using XFiresecAPI;

namespace GKModule
{
	public class Alarm
	{
		public XAlarmType AlarmType { get; set; }
		public XDevice Device { get; set; }
		public XZone Zone { get; set; }
		public XDirection Direction { get; set; }

		public Alarm(XAlarmType alarmType, XDevice device)
		{
			AlarmType = alarmType;
			Device = device;
		}

		public Alarm(XAlarmType alarmType, XZone zone)
		{
			AlarmType = alarmType;
			Zone = zone;
		}

		public Alarm(XAlarmType alarmType, XDirection direction)
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