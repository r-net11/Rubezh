using System.Runtime.Serialization;

namespace StrazhAPI.Automation
{
	[DataContract]
	public class ForArguments
	{
		public ForArguments()
		{
			IndexerArgument = new Argument();
			InitialValueArgument = new Argument();
			ValueArgument = new Argument();
			IteratorArgument = new Argument();
			ConditionType = ConditionType.IsLess;
		}

		[DataMember]
		public Argument IndexerArgument { get; set; }

		[DataMember]
		public Argument InitialValueArgument { get; set; }

		[DataMember]
		public Argument ValueArgument { get; set; }

		[DataMember]
		public Argument IteratorArgument { get; set; }

		[DataMember]
		public ConditionType ConditionType { get; set; }
	}
}