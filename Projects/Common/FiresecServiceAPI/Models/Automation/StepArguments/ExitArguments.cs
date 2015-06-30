using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class ExitArguments
	{
		public ExitArguments()
		{
			ExitCodeArgument = new Argument();
		}

		[DataMember]
		public Argument ExitCodeArgument { get; set; }
	}
}