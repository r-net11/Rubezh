using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class GetObjectPropertyStep : ProcedureStep
	{
		public GetObjectPropertyStep()
		{
			ObjectArgument = new Argument();
			ResultArgument = new Argument();
		}

		[DataMember]
		public Argument ObjectArgument { get; set; }

		[DataMember]
		public Argument ResultArgument { get; set; }

		[DataMember]
		public Property Property { get; set; }

		[DataMember]
		public ObjectType ObjectType { get; set; }

		public override ProcedureStepType ProcedureStepType { get { return ProcedureStepType.GetObjectProperty; } }
	}
}