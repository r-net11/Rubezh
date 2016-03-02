using System.ComponentModel;
using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class ControlDirectionArguments
	{
		public ControlDirectionArguments()
		{
			DirectionArgument = new Argument();
		}

		[DataMember]
		public Argument DirectionArgument { get; set; }

		[DataMember]
		public DirectionCommandType DirectionCommandType { get; set; }
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