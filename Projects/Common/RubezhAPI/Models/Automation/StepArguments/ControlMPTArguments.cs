using System.ComponentModel;
using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class ControlMPTArguments
	{
		public ControlMPTArguments()
		{
			MPTArgument = new Argument();
		}

		[DataMember]
		public Argument MPTArgument { get; set; }

		[DataMember]
		public MPTCommandType MPTCommandType { get; set; }
	}

	public enum MPTCommandType
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