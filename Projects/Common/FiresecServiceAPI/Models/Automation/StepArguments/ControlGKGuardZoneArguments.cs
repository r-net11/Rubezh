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
		
		[Description("Поставить на охрану немедленно")]
		TurnOnNow,

		[Description("Снять с охраны")]
		TurnOff,
		
		[Description("Сбросить")]
		Reset
	}
}