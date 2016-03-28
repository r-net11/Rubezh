using System.Collections.Generic;
using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public abstract class ConditionStep : ProcedureStep
	{
		public ConditionStep()
		{
			Conditions = new List<Condition>();
		}

		[DataMember]
		public List<Condition> Conditions { get; set; }

		[DataMember]
		public JoinOperator JoinOperator { get; set; }
	}

	[DataContract]
	public class IfStep : ConditionStep { public override ProcedureStepType ProcedureStepType { get { return ProcedureStepType.If; } } }

	[DataContract]
	public class IfYesStep : ProcedureStep { public override ProcedureStepType ProcedureStepType { get { return ProcedureStepType.IfYes; } } }

	[DataContract]
	public class IfNoStep : ProcedureStep { public override ProcedureStepType ProcedureStepType { get { return ProcedureStepType.IfNo; } } }

	[DataContract]
	public class WhileStep : ConditionStep { public override ProcedureStepType ProcedureStepType { get { return ProcedureStepType.While; } } }
}