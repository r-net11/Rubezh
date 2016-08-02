using System.ComponentModel;
using System.Runtime.Serialization;

namespace StrazhAPI.Automation
{
	[DataContract]
	public class ControlDoorArguments
	{
		public ControlDoorArguments()
		{
			DoorArgument = new Argument();
		}

		[DataMember]
		public Argument DoorArgument { get; set; }

		[DataMember]
		public DoorCommandType DoorCommandType { get; set; }
	}

	public enum DoorCommandType
	{
		[Description("Открыть дверь")]
		Open,

		[Description("Закрыть дверь")]
		Close,

		[Description("Установить режим \"Открыто\"")]
		AccessStateOpenAlways,

		[Description("Установить режим \"Норма\"")]
		AccessStateNormal,

		[Description("Установить режим \"Закрыто\"")]
		AccessStateCloseAlways,

		[Description("Сброс состояния \"Взлом\"")]
		ClearPromptWarning
	}
}