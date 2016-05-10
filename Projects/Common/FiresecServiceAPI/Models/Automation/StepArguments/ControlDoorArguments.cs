using System.ComponentModel;
using System.Runtime.Serialization;
using LocalizationConveters;

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
		//[Description("Открыть дверь")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.ControlDoorArguments), "Open")]
		Open,

		//[Description("Закрыть дверь")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.ControlDoorArguments), "Close")]
        Close,

		//[Description("Установить режим \"Открыто\"")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.ControlDoorArguments), "AccessStateOpenAlways")]
        AccessStateOpenAlways,

		//[Description("Установить режим \"Норма\"")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.ControlDoorArguments), "AccessStateNormal")]
        AccessStateNormal,

		//[Description("Установить режим \"Закрыто\"")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.ControlDoorArguments), "AccessStateCloseAlways")]
        AccessStateCloseAlways,

		//[Description("Сброс состояния \"Взлом\"")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.ControlDoorArguments), "ClearPromptWarning")]
        ClearPromptWarning
	}
}