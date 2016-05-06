using System.Runtime.Serialization;

namespace StrazhAPI.Automation
{
	[DataContract]
	public class ForeachArguments
	{
		public ForeachArguments()
		{
			ListArgument = new Argument();
			ItemArgument = new Argument();
		}

		[DataMember]
		public Argument ListArgument { get; set; }

		[DataMember]
		public Argument ItemArgument { get; set; }
	}
}