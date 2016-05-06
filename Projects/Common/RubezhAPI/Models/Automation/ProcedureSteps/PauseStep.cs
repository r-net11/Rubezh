using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class PauseStep : ProcedureStep
	{
		public PauseStep()
		{
			PauseArgument = new Argument();
			PauseArgument.IntValue = 1;
		}

		[DataMember]
		public TimeType TimeType { get; set; }

		[DataMember]
		public Argument PauseArgument { get; set; }

		public override ProcedureStepType ProcedureStepType { get { return ProcedureStepType.Pause; } }

		public override Argument[] Arguments
		{
			get { return new Argument[] { PauseArgument }; }
		}
	}
}