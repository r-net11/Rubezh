using System.ComponentModel;
using System.Runtime.Serialization;

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
		[Description("Открыть")]
		Open,

		[Description("Закрыть")]
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