using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class ForeachStep : ProcedureStep
	{
		public ForeachStep()
		{
			ListArgument = new Argument();
			ItemArgument = new Argument();
		}

		[DataMember]
		public Argument ListArgument { get; set; }

		[DataMember]
		public Argument ItemArgument { get; set; }

		public override ProcedureStepType ProcedureStepType { get { return ProcedureStepType.Foreach; } }

		public override Argument[] Arguments
		{
			get { return new Argument[] { ListArgument, ItemArgument }; }
		}
	}

	[DataContract]
	public class ForeachBodyStep : ProcedureStep { public override ProcedureStepType ProcedureStepType { get { return ProcedureStepType.ForeachBody; } } }
}