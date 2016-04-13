using RubezhAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace RubezhAPI.Automation
{
	[DataContract]
	public class AutomationConfiguration
	{
		public AutomationConfiguration()
		{
			Procedures = new List<Procedure>();
			AutomationSchedules = new List<AutomationSchedule>();
			AutomationSounds = new List<AutomationSound>();
			OPCServers = new List<OPCServer>();
			GlobalVariables = new List<Variable>();
			OpcDaTagFilters = new List<OpcDaTagFilter>();
			OpcDaTsServers = new List<OpcDaServer>();
		}

		[DataMember]
		public List<Procedure> Procedures { get; set; }

		[DataMember]
		public List<AutomationSchedule> AutomationSchedules { get; set; }

		[DataMember]
		public List<AutomationSound> AutomationSounds { get; set; }

		[DataMember]
		public List<OPCServer> OPCServers { get; set; }

		[DataMember]
		public List<Variable> GlobalVariables { get; set; }

		[DataMember]
		public List<OpcDaServer> OpcDaTsServers { get; set; }

		[DataMember]
		public List<OpcDaTagFilter> OpcDaTagFilters { get; set; }

		public void UpdateConfiguration()
		{
			foreach (var procedure in Procedures)
			{
				foreach (var step in procedure.Steps)
				{
					InvalidateStep(step, procedure);
				}
			}

			foreach (var schedule in AutomationSchedules)
			{
				var tempScheduleProcedures = new List<ScheduleProcedure>();
				foreach (var scheduleProcedure in schedule.ScheduleProcedures)
				{
					var procedure = Procedures.FirstOrDefault(x => x.Uid == scheduleProcedure.ProcedureUid);
					if (procedure != null)
					{
						var tempArguments = new List<Argument>();
						int i = 0;
						foreach (var variable in procedure.Arguments)
						{
							Argument argument;
							if (scheduleProcedure.Arguments.Count <= i)
								argument = InitializeArgumemt(variable);
							else
							{
								if (!CheckSignature(scheduleProcedure.Arguments[i], variable))
									argument = InitializeArgumemt(variable);
								else
									argument = scheduleProcedure.Arguments[i];
							}
							tempArguments.Add(argument);
							i++;
						}
						scheduleProcedure.Arguments = new List<Argument>(tempArguments);
						tempScheduleProcedures.Add(scheduleProcedure);
					}
				}
				schedule.ScheduleProcedures = new List<ScheduleProcedure>(tempScheduleProcedures);
				foreach (var scheduleProcedure in schedule.ScheduleProcedures)
				{
					foreach (var argument in scheduleProcedure.Arguments)
						InvalidateArgument(argument);
				}
			}
		}

		Argument InitializeArgumemt(Variable variable)
		{
			var argument = new Argument();
			argument.VariableScope = VariableScope.GlobalVariable;
			argument.Value = variable.Value;
			return argument;
		}

		bool CheckSignature(Argument argument, Variable variable)
		{
			if (argument.ExplicitType != variable.ExplicitType)
				return false;
			if (argument.ExplicitType != ExplicitType.Object && argument.ExplicitType != ExplicitType.Enum)
				return true;
			if (argument.ExplicitType != ExplicitType.Object)
				return (argument.ObjectType == variable.ObjectType);
			if (argument.ExplicitType != ExplicitType.Enum)
				return (argument.EnumType == variable.EnumType);
			return false;
		}

		void InvalidateArguments(ProcedureStep step, Procedure procedure)
		{
			foreach (var argument in step.GetType().GetProperties().Where(x => x.PropertyType == typeof(Argument)))
			{
				var value = (Argument)argument.GetValue(step, null);
				InvalidateArgument(procedure, value);
			}
		}

		public void InvalidateStep(ProcedureStep step, Procedure procedure)
		{
			InvalidateArguments(step, procedure);

			switch (step.ProcedureStepType)
			{
				case ProcedureStepType.PlaySound:
					var soundStep = (SoundStep)step;
					if (AutomationSounds.All(x => x.Uid != soundStep.SoundUid))
						soundStep.SoundUid = Guid.Empty;
					break;

				case ProcedureStepType.If:
				case ProcedureStepType.While:
					var conditionStep = (ConditionStep)step;
					foreach (var condition in conditionStep.Conditions)
					{
						InvalidateArgument(procedure, condition.Argument1);
						InvalidateArgument(procedure, condition.Argument2);
					}
					break;

				case ProcedureStepType.FindObjects:
					var findObjectStep = (FindObjectStep)step;
					foreach (var findObjectCondition in findObjectStep.FindObjectConditions)
						InvalidateArgument(procedure, findObjectCondition.SourceArgument);
					break;

				case ProcedureStepType.ProcedureSelection:
					var procedureSelectionStep = (ProcedureSelectionStep)step;
					if (Procedures.All(x => x.Uid != procedureSelectionStep.ScheduleProcedure.ProcedureUid))
						procedureSelectionStep.ScheduleProcedure.ProcedureUid = Guid.Empty;
					foreach (var argument in procedureSelectionStep.ScheduleProcedure.Arguments)
						InvalidateArgument(procedure, argument);
					break;

				case ProcedureStepType.ControlPlanGet:
				case ProcedureStepType.ControlPlanSet:
					var controlPlanStep = (ControlPlanStep)step;
					if (ConfigurationCash.PlansConfiguration == null || ConfigurationCash.PlansConfiguration.AllPlans == null)
						return;
					var plan = ConfigurationCash.PlansConfiguration.AllPlans.FirstOrDefault(x => x.UID == controlPlanStep.PlanUid);
					if (plan == null)
					{
						controlPlanStep.PlanUid = Guid.Empty;
						controlPlanStep.ElementUid = Guid.Empty;
					}
					else
					{
						if (plan.AllElements.All(x => x.UID != controlPlanStep.ElementUid))
							controlPlanStep.ElementUid = Guid.Empty;
					}
					break;
				case ProcedureStepType.ControlOpcDaTagGet:
				case ProcedureStepType.ControlOpcDaTagSet:
					var controlOpcDaTagStep = (ControlOpcDaTagStep)step;
					var server = OpcDaTsServers.FirstOrDefault(x => x.Uid == controlOpcDaTagStep.OpcDaServerUID);
					if (server == null)
					{
						controlOpcDaTagStep.OpcDaServerUID = Guid.Empty;
						controlOpcDaTagStep.OpcDaTagUID = Guid.Empty;
					}
					else if (server.Tags.All(x => x.Uid != controlOpcDaTagStep.OpcDaTagUID))
						controlOpcDaTagStep.OpcDaTagUID = Guid.Empty;
					break;

			}
		}

		void InvalidateArgument(Procedure procedure, Argument argument)
		{
			var localVariables = new List<Variable>(procedure.Variables);
			localVariables.AddRange(new List<Variable>(procedure.Arguments));
			if (argument.VariableScope == VariableScope.GlobalVariable)
				if (GlobalVariables.All(x => x.Uid != argument.VariableUid))
					argument.VariableUid = Guid.Empty;
			if (argument.VariableScope == VariableScope.LocalVariable)
				if (localVariables.All(x => x.Uid != argument.VariableUid))
					argument.VariableUid = Guid.Empty;
		}

		void InvalidateArgument(Argument argument)
		{
			if (argument.VariableScope != VariableScope.ExplicitValue)
				if (GlobalVariables.All(x => x.Uid != argument.VariableUid))
					argument.VariableUid = Guid.Empty;
		}
	}
}