using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace FiresecAPI.Automation
{
	[DataContract]
	public class AutomationConfiguration
	{
		public AutomationConfiguration()
		{
			Procedures = new List<Procedure>();
			AutomationSchedules = new List<AutomationSchedule>();
			AutomationSounds = new List<AutomationSound>();
			GlobalVariables = new List<GlobalVariable>();
		}

		[DataMember]
		public List<Procedure> Procedures { get; set; }

		[DataMember]
		public List<AutomationSchedule> AutomationSchedules { get; set; }

		[DataMember]
		public List<AutomationSound> AutomationSounds { get; set; }

		[DataMember]
		public List<GlobalVariable> GlobalVariables { get; set; }

		public void UpdateConfiguration() //TODO???
		{
			foreach (var procedure in Procedures)
			{
				foreach (var step in procedure.Steps)
				{
					UpdateStep(step, procedure);
				}
			}
		}

		public void UpdateStep(ProcedureStep step, Procedure procedure)
		{
			/* ArithmeticArguments */
			if (GlobalVariables.All(x => x.Uid != step.ArithmeticArguments.Variable1.GlobalVariableUid))
				step.ArithmeticArguments.Variable1.GlobalVariableUid = Guid.Empty;
			if (procedure.Variables.All(x => x.Uid != step.ArithmeticArguments.Variable1.VariableUid))
				step.ArithmeticArguments.Variable1.VariableUid = Guid.Empty;
			if (GlobalVariables.All(x => x.Uid != step.ArithmeticArguments.Variable2.GlobalVariableUid))
				step.ArithmeticArguments.Variable2.GlobalVariableUid = Guid.Empty;
			if (procedure.Variables.All(x => x.Uid != step.ArithmeticArguments.Variable2.VariableUid))
				step.ArithmeticArguments.Variable2.VariableUid = Guid.Empty;
			if (GlobalVariables.All(x => x.Uid != step.ArithmeticArguments.Result.GlobalVariableUid))
				step.ArithmeticArguments.Result.GlobalVariableUid = Guid.Empty;
			if (procedure.Variables.All(x => x.Uid != step.ArithmeticArguments.Result.VariableUid))
				step.ArithmeticArguments.Result.VariableUid = Guid.Empty;

			/* ConditionArguments */
			foreach (var condition in step.ConditionArguments.Conditions)
			{
				if (GlobalVariables.All(x => x.Uid != condition.Variable1.GlobalVariableUid))
					condition.Variable1.GlobalVariableUid = Guid.Empty;
				if (procedure.Variables.All(x => x.Uid != condition.Variable1.VariableUid))
					condition.Variable1.VariableUid = Guid.Empty;
				if (GlobalVariables.All(x => x.Uid != condition.Variable2.GlobalVariableUid))
					condition.Variable2.GlobalVariableUid = Guid.Empty;
				if (procedure.Variables.All(x => x.Uid != condition.Variable2.VariableUid))
					condition.Variable2.VariableUid = Guid.Empty;
			}
		}
	}
}