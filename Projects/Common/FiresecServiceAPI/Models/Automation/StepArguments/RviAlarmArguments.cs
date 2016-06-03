using System.Runtime.Serialization;

namespace StrazhAPI.Automation
{
	[DataContract]
	public class RviAlarmArguments
	{
		public RviAlarmArguments()
		{
			NameArgument = new Argument {ExplicitValue = {StringValue = string.Empty}};
		}

		[DataMember]
		public Argument NameArgument { get; set; }
	}
}