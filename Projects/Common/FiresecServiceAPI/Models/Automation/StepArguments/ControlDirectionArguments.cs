using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract, Serializable]
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
		
		[Description("Отключение")]
		Ignore,
		
		[Description("Пуск")]
		TurnOn,

		[Description("Пуск немедленно")]
		TurnOnNow,

		[Description("Останов пуска")]
		ForbidStart,

		[Description("Стоп немедленно")]
		TurnOff
	}
}