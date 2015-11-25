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
		
		[Description("Включить")]
		TurnOn,

		[Description("Стоп")]
		Stop,

		[Description("Останов пуска")]
		ForbidStart,

		[Description("Выключить")]
		TurnOff
	}
}