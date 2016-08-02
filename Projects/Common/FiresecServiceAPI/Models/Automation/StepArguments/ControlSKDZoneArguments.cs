using System.ComponentModel;
using System.Runtime.Serialization;

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
		[Description("Открыть все двери")]
		Open,

		[Description("Закрыть все двери")]
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