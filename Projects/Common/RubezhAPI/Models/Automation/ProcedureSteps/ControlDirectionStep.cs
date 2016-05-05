using System.ComponentModel;
using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class ControlDirectionStep : ProcedureStep
	{
		public ControlDirectionStep()
		{
			DirectionArgument = new Argument();
		}

		[DataMember]
		public Argument DirectionArgument { get; set; }

		[DataMember]
		public DirectionCommandType DirectionCommandType { get; set; }

		public override ProcedureStepType ProcedureStepType { get { return ProcedureStepType.ControlDirection; } }

		public override Argument[] Arguments
		{
			get { return new Argument[] { DirectionArgument }; }
		}
	}

	public enum DirectionCommandType
	{
		[Description("Автоматика")]
		Automatic,

		[Description("Ручное")]
		Manual,

		[Description("Отключить")]
		Ignore,

		[Description("Пуск")]
		TurnOn,

		[Description("Пуск немедленно")]
		TurnOnNow,

		[Description("Выключить")]
		TurnOff,

		[Description("Пуск в автоматическом режиме")]
		TurnOn_InAutomatic,

		[Description("Пуск немедленно в автоматическом режиме")]
		TurnOnNow_InAutomatic,

		[Description("Выключить в автоматическом режиме")]
		TurnOff_InAutomatic,

		[Description("Останов пуска")]
		ForbidStart
	}
}