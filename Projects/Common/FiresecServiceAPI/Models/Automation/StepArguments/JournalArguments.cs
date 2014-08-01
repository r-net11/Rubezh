using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	public class JournalArguments
	{
		public JournalArguments()
		{

		}

		[DataMember]
		public string Message { get; set; }
	}
}