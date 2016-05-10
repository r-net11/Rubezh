using System.Runtime.Serialization;

namespace StrazhAPI.Automation
{
	[DataContract]
	public class StartRecordArguments
	{
		public StartRecordArguments()
		{
			CameraArgument = new Argument();
			EventUIDArgument = new Argument();
			TimeoutArgument = new Argument();
		}

		[DataMember]
		public Argument CameraArgument { get; set; }

		[DataMember]
		public Argument EventUIDArgument { get; set; }

		[DataMember]
		public Argument TimeoutArgument { get; set; }
	}
}