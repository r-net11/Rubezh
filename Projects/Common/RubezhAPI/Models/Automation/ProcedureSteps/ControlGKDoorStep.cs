using Common;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class ControlGKDoorStep : ProcedureStep
	{
		public ControlGKDoorStep()
		{
			DoorArgument = new Argument();
		}

		[DataMember]
		public Argument DoorArgument { get; set; }

		[DataMember]
		public SafeEnum<GKDoorCommandType> DoorCommandType { get; set; }

		public override ProcedureStepType ProcedureStepType { get { return ProcedureStepType.ControlGKDoor; } }
	}

	public enum GKDoorCommandType
	{
		[Description("Автоматика")]
		Automatic,

		[Description("Ручное")]
		Manual,

		[Description("Отключить")]
		Ignore,

		[Description("Открыть дверь")]
		Open,

		[Description("Закрыть дверь")]
		Close,

		[Description("Закрыть дверь сразу")]
		CloseNow,

		[Description("Открыть дверь в автоматическом режиме")]
		OpenInAutomatic,

		[Description("Закрыть дверь в автоматическом режиме")]
		CloseInAutomatic,

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