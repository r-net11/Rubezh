using System.Runtime.Serialization;

namespace RubezhAPI.Automation
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