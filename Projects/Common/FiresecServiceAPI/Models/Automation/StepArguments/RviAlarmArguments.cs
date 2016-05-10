using System.Runtime.Serialization;

namespace StrazhAPI.Automation
{
	[DataContract]
	public class RviAlarmArguments
	{
		public RviAlarmArguments()
		{
			NameArgument = new Argument();
			NameArgument.ExplicitValue.StringValue = "";
		}

		[DataMember]
		public Argument NameArgument { get; set; }
	}
}