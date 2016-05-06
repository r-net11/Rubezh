using System.ComponentModel;
using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class ControlDelayStep : ProcedureStep
	{
		public ControlDelayStep()
		{
			DelayArgument = new Argument();
		}

		[DataMember]
		public Argument DelayArgument { get; set; }

		[DataMember]
		public DelayCommandType DelayCommandType { get; set; }

		public override ProcedureStepType ProcedureStepType { get { return ProcedureStepType.ControlDelay; } }

		public override Argument[] Arguments
		{
			get { return new Argument[] { DelayArgument }; }
		}
	}

	public enum DelayCommandType
	{
		[Description("Автоматика")]
		Automatic,

		[Description("Ручное")]
		Manual,

		[Description("Отключение")]
		Ignore,

		[Description("Включить")]
		TurnOn,

		[Description("Включить немедленно")]
		TurnOnNow,

		[Description("Выключить")]
		TurnOff,

		[Description("Включить в автоматическом режиме")]
		TurnOn_InAutomatic,

		[Description("Включить немедленно в автоматическом режиме")]
		TurnOnNow_InAutomatic,

		[Description("Выключить в автоматическом режиме")]
		TurnOff_InAutomatic
	}
}