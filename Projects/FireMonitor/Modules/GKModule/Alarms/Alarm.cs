using FiresecAPI.GK;

namespace GKModule
{
	public class Alarm
	{
		public GKAlarmType AlarmType { get; set; }
		public GKDevice Device { get; set; }
		public GKDirection Direction { get; set; }
		public GKDoor Door { get; set; }
		public GKMPT Mpt { get; set; }

		public Alarm(GKAlarmType alarmType, GKDevice device)
		{
			AlarmType = alarmType;
			Device = device;
		}

		public Alarm(GKAlarmType alarmType, GKDirection direction)
		{
			AlarmType = alarmType;
			Direction = direction;
		}

		public Alarm(GKAlarmType alarmType, GKDoor door)
		{
			AlarmType = alarmType;
			Door = door;
		}

		public Alarm(GKAlarmType alarmType, GKMPT mpt)
		{
			AlarmType = alarmType;
			Mpt = mpt;
		}

		public Alarm Clone()
		{
			var alarm = new Alarm(AlarmType, Device);
			alarm.Direction = Direction;
			return alarm;
		}

		public bool IsEqualTo(Alarm alarm)
		{
			if (alarm.AlarmType != AlarmType)
				return false;
			if (alarm.Device != null && alarm.Device != Device)
				return false;
			if (alarm.Direction != null && alarm.Direction != Direction)
				return false;
			return true;
		}
	}
}