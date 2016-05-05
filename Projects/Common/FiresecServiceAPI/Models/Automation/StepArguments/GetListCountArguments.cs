using System.Runtime.Serialization;

namespace StrazhAPI.Automation
{
	[DataContract]
	public class GetListCountArguments
	{
		public GetListCountArguments()
		{
			ListArgument = new Argument();
			CountArgument = new Argument();
		}

		[DataMember]
		public Argument ListArgument { get; set; }

		[DataMember]
		public Argument CountArgument { get; set; }
	}
}