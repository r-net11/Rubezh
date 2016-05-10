using System.ComponentModel;
using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class ControlMPTStep : ProcedureStep
	{
		public ControlMPTStep()
		{
			MPTArgument = new Argument();
		}

		[DataMember]
		public Argument MPTArgument { get; set; }

		[DataMember]
		public MPTCommandType MPTCommandType { get; set; }

		public override ProcedureStepType ProcedureStepType { get { return ProcedureStepType.ControlMPT; } }

		public override Argument[] Arguments
		{
			get { return new Argument[] { MPTArgument }; }
		}
	}

	public enum MPTCommandType
	{
		[Description("Автоматика")]
		Automatic,

		[Description("Ручное")]
		Manual,

		[Description("Отключить")]
		Ignore,

		[Description("Пуск")]
		TurnOn,

		[Description("Выключить")]
		TurnOff,

		[Description("Пуск немедленно")]
		TurnOnNow,

		[Description("Пуск в автоматическом режиме")]
		TurnOn_InAutomatic,

		[Description("Выключить в автоматическом режиме")]
		TurnOff_InAutomatic,

		[Description("Пуск немедленно в автоматическом режиме")]
		TurnOnNow_InAutomatic,

		[Description("Останов пуска")]
		ForbidStart
	}
}