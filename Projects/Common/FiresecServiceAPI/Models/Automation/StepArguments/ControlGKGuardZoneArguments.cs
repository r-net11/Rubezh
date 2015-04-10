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
			GKGuardZoneArgument = new Argument();
		}

		[DataMember]
		public Argument GKGuardZoneArgument { get; set; }

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

		[Description("Снять с охраны")]
		TurnOff,
		
		[Description("Поставить на охрану немедленно")]
		TurnOnNow,

		[Description("Снять с охраны немедленно")]
		TurnOffNow,

		[Description("Поставить на охрану в автоматическом режиме")]
		TurnOnInAutomatic,

		[Description("Снять с охраны в автоматическом режиме")]
		TurnOffInAutomatic,

		[Description("Поставить на охрану немедленно в автоматическом режиме")]
		TurnOnNowInAutomatic,

		[Description("Снять с охраны немедленно в автоматическом режиме")]
		TurnOffNowInAutomatic,
		
		[Description("Сбросить")]
		Reset
	}
}