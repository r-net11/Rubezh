using System.Runtime.Serialization;

namespace StrazhAPI.Automation
{
	[DataContract]
	public class PtzArguments
	{
		public PtzArguments()
		{
			CameraArgument = new Argument();
			PtzNumberArgument = new Argument();
		}

		[DataMember]
		public Argument CameraArgument { get; set; }

		[DataMember]
		public Argument PtzNumberArgument { get; set; }
	}
}