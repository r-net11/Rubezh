using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class ProcedureSelectionStep : ProcedureStep
	{
		public ProcedureSelectionStep()
		{
			ScheduleProcedure = new ScheduleProcedure();
		}

		[DataMember]
		public ScheduleProcedure ScheduleProcedure { get; set; }

		public override ProcedureStepType ProcedureStepType { get { return ProcedureStepType.ProcedureSelection; } }
	}
}