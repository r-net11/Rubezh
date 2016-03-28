using System.ComponentModel;
using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class ControlGKFireZoneStep : ProcedureStep
	{
		public ControlGKFireZoneStep()
		{
			GKFireZoneArgument = new Argument();
		}

		[DataMember]
		public Argument GKFireZoneArgument { get; set; }

		[DataMember]
		public ZoneCommandType ZoneCommandType { get; set; }

		public override ProcedureStepType ProcedureStepType { get { return ProcedureStepType.ControlGKFireZone; } }
	}

	public enum ZoneCommandType
	{
		[Description("Отключить")]
		Ignore,

		[Description("Снять отключение")]
		ResetIgnore,

		[Description("Сбросить")]
		Reset
	}
}