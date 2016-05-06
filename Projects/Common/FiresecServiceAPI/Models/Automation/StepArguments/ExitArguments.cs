using System.Runtime.Serialization;

namespace StrazhAPI.Automation
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