using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class GenerateGuidArguments
	{
		public GenerateGuidArguments()
		{
			ResultArgument = new Argument();
		}

		[DataMember]
		public Argument ResultArgument{ get; set; }
	}
}
