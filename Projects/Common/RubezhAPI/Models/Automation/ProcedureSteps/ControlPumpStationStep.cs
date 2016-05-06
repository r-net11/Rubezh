using System.ComponentModel;
using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class ControlPumpStationStep : ProcedureStep
	{
		public ControlPumpStationStep()
		{
			PumpStationArgument = new Argument();
		}

		[DataMember]
		public Argument PumpStationArgument { get; set; }

		[DataMember]
		public PumpStationCommandType PumpStationCommandType { get; set; }

		public override ProcedureStepType ProcedureStepType { get { return ProcedureStepType.ControlPumpStation; } }

		public override Argument[] Arguments
		{
			get { return new Argument[] { PumpStationArgument }; }
		}
	}

	public enum PumpStationCommandType
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