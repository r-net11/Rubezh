using System.ComponentModel;
using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class ControlGKGuardZoneStep : ProcedureStep
	{
		public ControlGKGuardZoneStep()
		{
			GKGuardZoneArgument = new Argument();
		}

		[DataMember]
		public Argument GKGuardZoneArgument { get; set; }

		[DataMember]
		public GuardZoneCommandType GuardZoneCommandType { get; set; }

		public override ProcedureStepType ProcedureStepType { get { return ProcedureStepType.ControlGKGuardZone; } }

		public override Argument[] Arguments
		{
			get { return new Argument[] { GKGuardZoneArgument }; }
		}
	}

	public enum GuardZoneCommandType
	{
		[Description("Автоматика")]
		Automatic,

		[Description("Ручное")]
		Manual,

		[Description("Отключение")]
		Ignore,

		[Description("Включить")]
		TurnOn,

		[Description("Выключить")]
		TurnOff,

		[Description("Включить немедленно")]
		TurnOnNow,

		[Description("Выключить немедленно")]
		TurnOffNow,

		[Description("Включить в автоматическом режиме")]
		TurnOnInAutomatic,

		[Description("Выключить в автоматическом режиме")]
		TurnOffInAutomatic,

		[Description("Включить немедленно в автоматическом режиме")]
		TurnOnNowInAutomatic,

		[Description("Выключить немедленно в автоматическом режиме")]
		TurnOffNowInAutomatic,

		[Description("Сбросить")]
		Reset
	}
}