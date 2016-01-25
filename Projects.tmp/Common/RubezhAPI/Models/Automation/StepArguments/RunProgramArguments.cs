using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class RunProgramArguments
	{
		public RunProgramArguments()
		{
			PathArgument = new Argument();
			ParametersArgument = new Argument();
		}

		[DataMember]
		public Argument PathArgument { get; set; }

		[DataMember]
		public Argument ParametersArgument { get; set; }
	}
}