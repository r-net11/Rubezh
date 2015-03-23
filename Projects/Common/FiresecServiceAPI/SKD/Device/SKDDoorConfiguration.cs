﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace FiresecAPI.SKD
{
	[DataContract]
	public class SKDDoorConfiguration
	{
		public SKDDoorConfiguration()
		{
			DoorOpenMethod = SKDDoorConfiguration_DoorOpenMethod.CFG_DOOR_OPEN_METHOD_CARD;
			UnlockHoldInterval = 2000;
			CloseTimeout = 0;
		}

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
		}

		[DataMember]
		public List<DoorDayIntervalPart> DoorDayIntervalParts { get; set; }
	}

	[DataContract]
	public class DoorDayIntervalPart
	{
		public DoorDayIntervalPart()
		{
			DoorOpenMethod = SKDDoorConfiguration_DoorOpenMethod.CFG_DOOR_OPEN_METHOD_CARD;
		}

		[DataMember]
		public int StartHour { get; set; }

		[DataMember]
		public int StartMinute { get; set; }

		[DataMember]
		public int EndHour { get; set; }

		[DataMember]
		public int EndMinute { get; set; }

		[DataMember]
		public SKDDoorConfiguration_DoorOpenMethod DoorOpenMethod { get; set; }
	}
}