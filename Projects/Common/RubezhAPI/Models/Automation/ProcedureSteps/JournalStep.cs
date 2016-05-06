using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class JournalStep : ProcedureStep
	{
		public JournalStep()
		{
			MessageArgument = new Argument();
		}

		[DataMember]
		public Argument MessageArgument { get; set; }

		[DataMember]
		public ExplicitType ExplicitType { get; set; }

		[DataMember]
		public EnumType EnumType { get; set; }

		[DataMember]
		public ObjectType ObjectType { get; set; }

		public override ProcedureStepType ProcedureStepType { get { return ProcedureStepType.AddJournalItem; } }

		public override Argument[] Arguments
		{
			get { return new Argument[] { MessageArgument }; }
		}
	}
}