using System.ComponentModel;
using System.Runtime.Serialization;
using LocalizationConveters;

namespace StrazhAPI.Automation
{
	[DataContract]
	public class ControlSKDZoneArguments
	{
		public ControlSKDZoneArguments()
		{
			SKDZoneArgument = new Argument();
		}

		[DataMember]
		public Argument SKDZoneArgument { get; set; }

		[DataMember]
		public SKDZoneCommandType SKDZoneCommandType { get; set; }
	}

	public enum SKDZoneCommandType
	{
		//[Description("Открыть все двери")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.ControlSKDZoneArguments), "Open")]
        Open,

		//[Description("Закрыть все двери")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.ControlSKDZoneArguments), "Close")]
        Close,

		//[Description("Установить режим \"Открыто\"")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.ControlSKDZoneArguments), "")]
        AccessStateOpenAlways,

		//[Description("Установить режим \"Норма\"")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.ControlSKDZoneArguments), "AccessStateNormal")]
        AccessStateNormal,

		//[Description("Установить режим \"Закрыто\"")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.ControlSKDZoneArguments), "AccessStateCloseAlways")]
        AccessStateCloseAlways,

		//[Description("Сброс состояния \"Взлом\"")]
        [LocalizedDescription(typeof(Resources.Language.Models.Automation.StepArguments.ControlSKDZoneArguments), "ClearPromptWarning")]
        ClearPromptWarning
	}
}