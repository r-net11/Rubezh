using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class ForStep : ProcedureStep
	{
		public ForStep()
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

		public override ProcedureStepType ProcedureStepType { get { return ProcedureStepType.For; } }

		public override Argument[] Arguments
		{
			get { return new Argument[] { IndexerArgument, InitialValueArgument, ValueArgument, IteratorArgument }; }
		}
	}
}
