using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class JournalArguments
	{
		public JournalArguments()
		{
			Variable1 = new ArithmeticParameter();
		}

		[DataMember]
		public ArithmeticParameter Variable1 { get; set; }

		[DataMember]
		public ExplicitType ExplicitType { get; set; }
	}
}