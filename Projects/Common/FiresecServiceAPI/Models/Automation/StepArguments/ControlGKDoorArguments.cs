using System.Runtime.Serialization;

namespace FiresecAPI.Automation
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
		public DoorCommandType DoorCommandType { get; set; }
	}
}