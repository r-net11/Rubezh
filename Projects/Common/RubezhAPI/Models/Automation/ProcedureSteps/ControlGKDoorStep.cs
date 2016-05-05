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
		[Description("Перевести в автоматический режим")]
		Automatic,

		[Description("Перевести в ручной режим")]
		Manual,

		[Description("Перевести в отключенный режим")]
		Ignore,

		[Description("Включить")]
		Open,

		[Description("Выключить")]
		Close,

		[Description("Выключить немедленно")]
		CloseNow,

		[Description("Включить в автоматическом режиме")]
		OpenInAutomatic,

		[Description("Выключить в автоматическом режиме")]
		CloseInAutomatic,

		[Description("Выключить немедленно в автоматическом режиме")]
		CloseNowInAutomatic,

		[Description("Установить режим ОТКРЫТО")]
		OpenForever,

		[Description("Установить режим НОРМА")]
		Norm,

		[Description("Установить режим ЗАКРЫТО")]
		CloseForever
	}
}