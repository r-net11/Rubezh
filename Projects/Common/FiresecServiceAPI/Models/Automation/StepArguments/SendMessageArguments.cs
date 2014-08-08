using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	public class SendMessageArguments
	{
		public SendMessageArguments()
		{

		}

		[DataMember]
		public string Message { get; set; }
	}
}