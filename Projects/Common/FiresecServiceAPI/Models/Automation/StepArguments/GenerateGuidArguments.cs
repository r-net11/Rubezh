using System.Runtime.Serialization;

namespace StrazhAPI.Automation
{
	[DataContract]
	public class GenerateGuidArguments
	{
		public GenerateGuidArguments()
		{
			ResultArgument = new Argument();
		}

		[DataMember]
		public Argument ResultArgument { get; set; }
	}
}