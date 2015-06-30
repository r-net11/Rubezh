using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class JournalArguments
	{
		public JournalArguments()
		{
			MessageArgument = new Argument();
		}

		[DataMember]
		public Argument MessageArgument { get; set; }

		[DataMember]
		public ExplicitType ExplicitType { get; set; }

		[DataMember]
		public EnumType EnumType { get; set; }
	}
}