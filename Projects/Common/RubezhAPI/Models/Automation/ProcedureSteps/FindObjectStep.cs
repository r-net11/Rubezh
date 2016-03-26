using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class FindObjectStep : ProcedureStep
	{
		public FindObjectStep()
		{
			FindObjectConditions = new List<FindObjectCondition>();
			ResultArgument = new Argument();
		}

		[DataMember]
		public List<FindObjectCondition> FindObjectConditions { get; set; }

		[DataMember]
		public JoinOperator JoinOperator { get; set; }

		[DataMember]
		public Argument ResultArgument { get; set; }

		public override ProcedureStepType ProcedureStepType { get { return ProcedureStepType.FindObjects; } }
	}
}