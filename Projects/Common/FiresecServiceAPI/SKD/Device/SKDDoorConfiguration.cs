using System.Runtime.Serialization;
using System.ComponentModel;
using System.Collections.Generic;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class SKDDoorConfiguration
	{
		public SKDDoorConfiguration()
		{
			AccessMode = SKDDoorConfiguration_AccessMode.ACCESS_MODE_HANDPROTECTED;
			AccessState = SKDDoorConfiguration_AccessState.ACCESS_STATE_NORMAL;
			DoorOpenMethod = SKDDoorConfiguration_DoorOpenMethod.CFG_DOOR_OPEN_METHOD_CARD;
			DoorDayIntervalsCollection = new DoorDayIntervalsCollection();
		}

		[DataMember]
		public SKDDoorConfiguration_AccessMode AccessMode { get; set; }

		[DataMember]
		public SKDDoorConfiguration_AccessState AccessState { get; set; }

		[DataMember]
		public SKDDoorConfiguration_DoorOpenMethod DoorOpenMethod { get; set; }

		[DataMember]
		public int UnlockHoldInterval { get; set; }

		[DataMember]
		public int CloseTimeout { get; set; }

		[DataMember]
		public int OpenAlwaysTimeIndex { get; set; }

		[DataMember]
		public int HolidayTimeRecoNo { get; set; }

		[DataMember]
		public bool IsBreakInAlarmEnable { get; set; }

		[DataMember]
		public bool IsRepeatEnterAlarmEnable { get; set; }

		[DataMember]
		public bool IsDoorNotClosedAlarmEnable { get; set; }

		[DataMember]
		public bool IsDuressAlarmEnable { get; set; }

		[DataMember]
		public bool IsSensorEnable { get; set; }

		[DataMember]
		public DoorDayIntervalsCollection DoorDayIntervalsCollection { get; set; }
	}

	public enum SKDDoorConfiguration_AccessState
	{
		[Description("Норма")]
		ACCESS_STATE_NORMAL,

		[Description("Всегда открыто")]
		ACCESS_STATE_CLOSEALWAYS,

		[Description("Всегда закрыто")]
		ACCESS_STATE_OPENALWAYS,
	}

	public enum SKDDoorConfiguration_AccessMode
	{
		[Description("HANDPROTECTED")]
		ACCESS_MODE_HANDPROTECTED,

		[Description("SAFEROOM")]
		ACCESS_MODE_SAFEROOM,

		[Description("OTHER")]
		ACCESS_MODE_OTHER,
	}

	public enum SKDDoorConfiguration_DoorOpenMethod
	{
		[Description("Неизвестно")]
		CFG_DOOR_OPEN_METHOD_UNKNOWN = 0,

		[Description("Только пароль")]
		CFG_DOOR_OPEN_METHOD_PWD_ONLY,

		[Description("Карта")]
		CFG_DOOR_OPEN_METHOD_CARD,

		[Description("Пароль или карта")]
		CFG_DOOR_OPEN_METHOD_PWD_OR_CARD,

		[Description("Сначала карта")]
		CFG_DOOR_OPEN_METHOD_CARD_FIRST,

		[Description("Сначала пароль")]
		CFG_DOOR_OPEN_METHOD_PWD_FIRST,

		[Description("Недельный график")]
		CFG_DOOR_OPEN_METHOD_SECTION,
	}

	[DataContract]
	public class DoorDayIntervalsCollection
	{
		public DoorDayIntervalsCollection()
		{
			DoorDayIntervals = new List<DoorDayInterval>();
			for (int i = 0; i < 7; i++)
			{
				var doorWeeklyInterval = new DoorDayInterval();
				DoorDayIntervals.Add(doorWeeklyInterval);
			}
		}

		[DataMember]
		public List<DoorDayInterval> DoorDayIntervals { get; set; }
	}

	[DataContract]
	public class DoorDayInterval
	{
		public DoorDayInterval()
		{
			DoorDayIntervalParts = new List<DoorDayIntervalPart>();
			for (int i = 0; i < 4; i++)
			{
				var doorDayInterval = new DoorDayIntervalPart();
				DoorDayIntervalParts.Add(doorDayInterval);
			}
		}

		[DataMember]
		public List<DoorDayIntervalPart> DoorDayIntervalParts { get; set; }
	}

	[DataContract]
	public class DoorDayIntervalPart
	{
		[DataMember]
		public int StartHour { get; set; }

		[DataMember]
		public int StartMinute { get; set; }

		[DataMember]
		public int EndHour { get; set; }

		[DataMember]
		public int EndMinute { get; set; }
	}
}