using System.ComponentModel;
using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class ControlPumpStationArguments
	{
		public ControlPumpStationArguments()
		{
			PumpStationArgument = new Argument();
		}

		[DataMember]
		public Argument PumpStationArgument { get; set; }

		[DataMember]
		public PumpStationCommandType PumpStationCommandType { get; set; }
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

		[Description("Выключить немедленно")]
		TurnOffNow,

		[Description("Пуск в автоматическом режиме")]
		TurnOn_InAutomatic,

		[Description("Выключить в автоматическом режиме")]
		TurnOff_InAutomatic,

		[Description("Пуск немедленно в автоматическом режиме")]
		TurnOnNow_InAutomatic,

		[Description("Выключить немедленно в автоматическом режиме")]
		TurnOffNow_InAutomatic,

		[Description("Стоп")]
		Stop,

		[Description("Останов пуска")]
		ForbidStart
	}
}