using System.Runtime.Serialization;

namespace StrazhAPI.Automation
{
	[DataContract]
	public class CheckPermissionArguments
	{
		public CheckPermissionArguments()
		{
			PermissionArgument = new Argument();
			ResultArgument = new Argument();
		}

		[DataMember]
		public Argument PermissionArgument { get; set; }

		[DataMember]
		public Argument ResultArgument { get; set; }
	}
}