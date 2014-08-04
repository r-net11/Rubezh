using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class ControlGKGuardZoneArguments
	{
		public ControlGKGuardZoneArguments()
		{
			Uid = Guid.NewGuid();
		}

		[DataMember]
		public Guid Uid { get; set; }

		[DataMember]
		public Guid ZoneUid { get; set; }

		[DataMember]
		public GuardZoneCommandType GuardZoneCommandType { get; set; }
	}

	public enum GuardZoneCommandType
	{
		[Description("Автоматика")]
		Automatic,

		[Description("Ручное")]
		Manual,
		
		[Description("Отключение")]
		Ignore,
		
		[Description("Поставить на охрану")]
		TurnOn,
		
		[Description("Поставить на охрану немедленно")]
		TurnOnNow,

		[Description("Снять с охраны")]
		TurnOff,
		
		[Description("Сбросить")]
		Reset
	}
}