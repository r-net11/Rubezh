using System.ComponentModel;
using System.Runtime.Serialization;
using LocalizationConveters;

namespace StrazhAPI.Automation
{
	[DataContract]
	public class ControlSKDDeviceArguments
	{
		public ControlSKDDeviceArguments()
		{
			SKDDeviceArgument = new Argument();
		}

		[DataMember]
		public Argument SKDDeviceArgument { get; set; }

		[DataMember]
		public SKDDeviceCommandType Command { get; set; }
	}

	public enum SKDDeviceCommandType
	{
		//[Description("Открыть")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.ControlSKDDeviceArguments), "Open")]
		Open,

		//[Description("Закрыть")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.ControlSKDDeviceArguments), "Close")]
        Close,

		//[Description("Установить режим \"Открыто\"")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.ControlSKDDeviceArguments), "AccessStateOpenAlways")]
        AccessStateOpenAlways,

		//[Description("Установить режим \"Норма\"")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.ControlSKDDeviceArguments), "AccessStateNormal")]
        AccessStateNormal,

		//[Description("Установить режим \"Закрыто\"")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.ControlSKDDeviceArguments), "AccessStateCloseAlways")]
        AccessStateCloseAlways,

		//[Description("Сброс состояния \"Взлом\"")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.ControlSKDDeviceArguments), "ClearPromptWarning")]
        ClearPromptWarning
	}
}