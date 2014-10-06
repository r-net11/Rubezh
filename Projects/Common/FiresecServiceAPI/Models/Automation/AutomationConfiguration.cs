using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;
using System;

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
			GlobalVariables = new List<Variable>();
		}

		[DataMember]
		public List<Procedure> Procedures { get; set; }

		[DataMember]
		public List<AutomationSchedule> AutomationSchedules { get; set; }

		[DataMember]
		public List<AutomationSound> AutomationSounds { get; set; }

		[DataMember]
		public List<Variable> GlobalVariables { get; set; }

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
							var argument = new Argument();
							if (scheduleProcedure.Arguments.Count <= i)
								argument = InitializeArgumemt(variable);
							else
							{
								if (!CheckSignature(scheduleProcedure.Arguments[i], variable))
								{
									argument = InitializeArgumemt(variable);

								}
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
			argument.ExplicitType = variable.ExplicitType;
			argument.EnumType = variable.EnumType;
			argument.ObjectType = variable.ObjectType;
			PropertyCopy.Copy<ExplicitValue, ExplicitValue>(variable.DefaultExplicitValue, argument.ExplicitValue);
			argument.ExplicitValues = new List<ExplicitValue>();
			foreach (var defaultExplicitValues in variable.DefaultExplicitValues)
			{
				var explicitValue = new ExplicitValue();
				PropertyCopy.Copy<ExplicitValue, ExplicitValue>(defaultExplicitValues, explicitValue);
				argument.ExplicitValues.Add(explicitValue);
			}
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

		public void InvalidateStep(ProcedureStep step, Procedure procedure)
		{
			switch (step.ProcedureStepType)
			{
				case ProcedureStepType.PlaySound:
					{
						var soundArguments = step.SoundArguments;
						if (AutomationSounds.All(x => x.Uid != soundArguments.SoundUid))
							soundArguments.SoundUid = Guid.Empty;
					}
					break;

				case ProcedureStepType.ShowMessage:
					{
						var showMessageArguments = step.ShowMessageArguments;
						InvalidateArgument(procedure, showMessageArguments.MessageArgument);
					}
					break;

				case ProcedureStepType.Arithmetics:
					{
						var arithmeticArguments = step.ArithmeticArguments;
						InvalidateArgument(procedure, arithmeticArguments.Argument1);
						InvalidateArgument(procedure, arithmeticArguments.Argument2);
						InvalidateArgument(procedure, arithmeticArguments.ResultArgument);
					}
					break;

				case ProcedureStepType.If:
				case ProcedureStepType.While:
					{
						var conditionArguments = step.ConditionArguments;
						foreach (var condition in conditionArguments.Conditions)
						{
							InvalidateArgument(procedure, condition.Argument1);
							InvalidateArgument(procedure, condition.Argument2);
						}
					}
					break;

				case ProcedureStepType.AddJournalItem:
					{
						var journalArguments = step.JournalArguments;
						InvalidateArgument(procedure, journalArguments.MessageArgument);
					}
					break;

				case ProcedureStepType.FindObjects:
					{
						var findObjectArguments = step.FindObjectArguments;
						InvalidateArgument(procedure, findObjectArguments.ResultArgument);
						foreach (var findObjectCondition in findObjectArguments.FindObjectConditions)
						{
							InvalidateArgument(procedure, findObjectCondition.SourceArgument);
						}
					}
					break;

				case ProcedureStepType.Foreach:
					{
						var foreachArguments = step.ForeachArguments;
						InvalidateArgument(procedure, foreachArguments.ItemArgument);
						InvalidateArgument(procedure, foreachArguments.ListArgument);
					}
					break;

				case ProcedureStepType.For:
					{
						var forArguments = step.ForArguments;
						InvalidateArgument(procedure, forArguments.IndexerArgument);
						InvalidateArgument(procedure, forArguments.InitialValueArgument);
						InvalidateArgument(procedure, forArguments.ValueArgument);
						InvalidateArgument(procedure, forArguments.IteratorArgument);
					}
					break;

				case ProcedureStepType.Pause:
					{
						var pauseArguments = step.PauseArguments;
						InvalidateArgument(procedure, pauseArguments.PauseArgument);
					}
					break;

				case ProcedureStepType.ProcedureSelection:
					{
						var procedureSelectionArguments = step.ProcedureSelectionArguments;
						if (Procedures.All(x => x.Uid != procedureSelectionArguments.ScheduleProcedure.ProcedureUid))
							procedureSelectionArguments.ScheduleProcedure.ProcedureUid = Guid.Empty;
						foreach (var argument in procedureSelectionArguments.ScheduleProcedure.Arguments)
							InvalidateArgument(procedure, argument);
					}
					break;

				case ProcedureStepType.Exit:
					{
						var exitArguments = step.ExitArguments;
						InvalidateArgument(procedure, exitArguments.ExitCodeArgument);
					}
					break;

				case ProcedureStepType.SetValue:
					{
						var setValueArguments = step.SetValueArguments;
						InvalidateArgument(procedure, setValueArguments.SourceArgument);
						InvalidateArgument(procedure, setValueArguments.TargetArgument);
					}
					break;

				case ProcedureStepType.IncrementValue:
					{
						var incrementValueArguments = step.IncrementValueArguments;
						InvalidateArgument(procedure, incrementValueArguments.ResultArgument);
					}
					break;

				case ProcedureStepType.ControlGKDevice:
					{
						var controlGKDeviceArguments = step.ControlGKDeviceArguments;
						InvalidateArgument(procedure, controlGKDeviceArguments.GKDeviceArgument);
					}
					break;

				case ProcedureStepType.ControlSKDDevice:
					{
						var controlSKDDeviceArguments = step.ControlSKDDeviceArguments;
						InvalidateArgument(procedure, controlSKDDeviceArguments.SKDDeviceArgument);
					}
					break;

				case ProcedureStepType.ControlGKFireZone:
					{
						var controlGKFireZoneArguments = step.ControlGKFireZoneArguments;
						InvalidateArgument(procedure, controlGKFireZoneArguments.GKFireZoneArgument);
					}
					break;

				case ProcedureStepType.ControlGKGuardZone:
					{
						var controlGKGuardZoneArguments = step.ControlGKGuardZoneArguments;
						InvalidateArgument(procedure, controlGKGuardZoneArguments.GKGuardZoneArgument);
					}
					break;

				case ProcedureStepType.ControlDirection:
					{
						var controlDirectionArguments = step.ControlDirectionArguments;
						InvalidateArgument(procedure, controlDirectionArguments.DirectionArgument);
					}
					break;

				case ProcedureStepType.ControlDoor:
					{
						var controlDoorArguments = step.ControlDoorArguments;
						InvalidateArgument(procedure, controlDoorArguments.DoorArgument);
					}
					break;

				case ProcedureStepType.ControlSKDZone:
					{
						var controlSKDZoneArguments = step.ControlSKDZoneArguments;
						InvalidateArgument(procedure, controlSKDZoneArguments.SKDZoneArgument);
					}
					break;

				case ProcedureStepType.ControlCamera:
					{
						var controlCameraArguments = step.ControlCameraArguments;
						InvalidateArgument(procedure, controlCameraArguments.CameraArgument);
					}
					break;

				case ProcedureStepType.GetObjectProperty:
					{
						var getObjectPropertyArguments = step.GetObjectPropertyArguments;
						InvalidateArgument(procedure, getObjectPropertyArguments.ObjectArgument);
						InvalidateArgument(procedure, getObjectPropertyArguments.ResultArgument);
					}
					break;

				case ProcedureStepType.SendEmail:
					{
						var sendEmailArguments = step.SendEmailArguments;
						InvalidateArgument(procedure, sendEmailArguments.EMailAddressArgument);
						InvalidateArgument(procedure, sendEmailArguments.EMailContentArgument);
						InvalidateArgument(procedure, sendEmailArguments.EMailTitleArgument);
						InvalidateArgument(procedure, sendEmailArguments.HostArgument);
						InvalidateArgument(procedure, sendEmailArguments.LoginArgument);
						InvalidateArgument(procedure, sendEmailArguments.PasswordArgument);
						InvalidateArgument(procedure, sendEmailArguments.PortArgument);
					}
					break;

				case ProcedureStepType.RunProgramm:
					{
						var runProgrammArguments = step.RunProgrammArguments;
						InvalidateArgument(procedure, runProgrammArguments.ParametersArgument);
						InvalidateArgument(procedure, runProgrammArguments.PathArgument);
					}
					break;

				case ProcedureStepType.Random:
					{
						var randomArguments = step.RandomArguments;
						InvalidateArgument(procedure, randomArguments.MaxValueArgument);
					}
					break;

				case ProcedureStepType.ChangeList:
					{
						var changeListArguments = step.ChangeListArguments;
						InvalidateArgument(procedure, changeListArguments.ItemArgument);
						InvalidateArgument(procedure, changeListArguments.ListArgument);
					}
					break;

				case ProcedureStepType.GetListCount:
					{
						var getListCountArgument = step.GetListCountArgument;
						InvalidateArgument(procedure, getListCountArgument.ListArgument);
						InvalidateArgument(procedure, getListCountArgument.CountArgument);
					}
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