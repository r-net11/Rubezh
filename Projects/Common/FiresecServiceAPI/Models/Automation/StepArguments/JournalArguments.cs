using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	public class JournalArguments
	{
		public JournalArguments()
		{
			Variable = new ArithmeticParameter();
		}

		[DataMember]
		public ArithmeticParameter Variable { get; set; }

		[DataMember]
		public ValueType ValueType { get; set; }
	}
}