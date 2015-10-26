using System.ComponentModel;
using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class ControlGKDoorArguments
	{
		public ControlGKDoorArguments()
		{
			DoorArgument = new Argument();
		}

		[DataMember]
		public Argument DoorArgument { get; set; }

		[DataMember]
		public GKDoorCommandType DoorCommandType { get; set; }
	}

	public enum GKDoorCommandType
	{
		[Description("Открыть дверь")]
		Open,

		[Description("Закрыть дверь")]
		Close,

		[Description("Открыть дверь сразу")]
		OpenNow,

		[Description("Закрыть дверь сразу")]
		CloseNow,

		[Description("Открыть дверь в автоматическом режиме")]
		OpenInAutomatic,

		[Description("Закрыть дверь в автоматическом режиме")]
		CloseInAutomatic,

		[Description("Открыть дверь сразу в автоматическом режиме")]
		OpenNowInAutomatic,

		[Description("Закрыть дверь сразу в автоматическом режиме")]
		CloseNowInAutomatic,

		[Description("Установить режим ОТКРЫТО")]
		OpenForever,

		[Description("Установить режим НОРМА")]
		Norm,

		[Description("Установить режим ЗАКРЫТО")]
		CloseForever
	}
}