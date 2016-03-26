using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class SetValueStep : ProcedureStep
	{
		public SetValueStep()
		{
			SourceArgument = new Argument();
			TargetArgument = new Argument();
		}

		[DataMember]
		public Argument TargetArgument { get; set; }

		[DataMember]
		public Argument SourceArgument { get; set; }

		[DataMember]
		public ExplicitType ExplicitType { get; set; }

		[DataMember]
		public EnumType EnumType { get; set; }

		[DataMember]
		public ObjectType ObjectType { get; set; }

		public override ProcedureStepType ProcedureStepType { get { return ProcedureStepType.SetValue; } }

	}
}