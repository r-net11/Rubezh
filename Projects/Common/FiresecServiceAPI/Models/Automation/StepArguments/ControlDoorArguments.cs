using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class ControlDoorArguments
	{
		public ControlDoorArguments()
		{
			Uid = Guid.NewGuid();
		}

		[DataMember]
		public Guid Uid { get; set; }

		[DataMember]
		public Guid DoorUid { get; set; }

		[DataMember]
		public DoorCommandType DoorCommandType { get; set; }
	}

	public enum DoorCommandType
	{
		[Description("Открыть дверь")]
		Open,

		[Description("Закрыть дверь")]
		Close,

		[Description("Установить режим ОТКРЫТО")]
		OpenForever,

		[Description("Установить режим ЗАКРЫТО")]
		CloseForever
	}
}