using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class ShowPropertyStep : UIStep
	{
		public ShowPropertyStep()
		{
			ObjectArgument = new Argument();
		}

		[DataMember]
		public Argument ObjectArgument { get; set; }

		[DataMember]
		public ObjectType ObjectType { get; set; }

		public override ProcedureStepType ProcedureStepType { get { return ProcedureStepType.ShowProperty; } }

		public override Argument[] Arguments
		{
			get { return new Argument[] { ObjectArgument }; }
		}
	}
}