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
					UpdateStep(step, procedure);
					InvalidateStep(step, procedure);
				}
			}
		}

		public void UpdateStep(ProcedureStep step, Procedure procedure)
		{

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
						InvalidateArgument(procedure, showMessageArguments.MessageParameter);
					}
					break;

				case ProcedureStepType.Arithmetics:
					{
						var arithmeticArguments = step.ArithmeticArguments;
						InvalidateArgument(procedure, arithmeticArguments.Parameter1);
						InvalidateArgument(procedure, arithmeticArguments.Parameter2);
						InvalidateArgument(procedure, arithmeticArguments.ResultParameter);
					}
					break;

				case ProcedureStepType.If:
					{
						var conditionArguments = step.ConditionArguments;
						foreach (var condition in conditionArguments.Conditions)
						{
							InvalidateArgument(procedure, condition.Parameter1);
							InvalidateArgument(procedure, condition.Parameter2);
						}
					}
					break;

				case ProcedureStepType.AddJournalItem:
					{
						var journalArguments = step.JournalArguments;
						InvalidateArgument(procedure, journalArguments.MessageParameter);
					}
					break;

				case ProcedureStepType.FindObjects:
					{
						var findObjectArguments = step.FindObjectArguments;
						InvalidateArgument(procedure, findObjectArguments.ResultParameter);
						foreach (var findObjectCondition in findObjectArguments.FindObjectConditions)
						{
							InvalidateArgument(procedure, findObjectCondition.SourceParameter);
						}
					}
					break;

				case ProcedureStepType.Foreach:
					{
						var foreachArguments = step.ForeachArguments;
						InvalidateArgument(procedure, foreachArguments.ItemParameter);
						InvalidateArgument(procedure, foreachArguments.ListParameter);
					}
					break;

				case ProcedureStepType.Pause:
					{
						var pauseArguments = step.PauseArguments;
						InvalidateArgument(procedure, pauseArguments.PauseParameter);
					}
					break;

				case ProcedureStepType.ProcedureSelection:
					{
						var procedureSelectionArguments = step.ProcedureSelectionArguments;
						if (Procedures.All(x => x.Uid != procedureSelectionArguments.ScheduleProcedure.ProcedureUid))
							procedureSelectionArguments.ScheduleProcedure.ProcedureUid = Guid.Empty;
					}
					break;

				case ProcedureStepType.Exit:
					{
						var exitArguments = step.ExitArguments;
						InvalidateArgument(procedure, exitArguments.ExitCodeParameter);
					}
					break;

				case ProcedureStepType.SetValue:
					{
						var setValueArguments = step.SetValueArguments;
						InvalidateArgument(procedure, setValueArguments.SourceParameter);
						InvalidateArgument(procedure, setValueArguments.TargetParameter);
					}
					break;

				case ProcedureStepType.IncrementValue:
					{
						var incrementValueArguments = step.IncrementValueArguments;
						InvalidateArgument(procedure, incrementValueArguments.ResultParameter);
					}
					break;

				case ProcedureStepType.ControlGKDevice:
					{
						var controlGKDeviceArguments = step.ControlGKDeviceArguments;
						InvalidateArgument(procedure, controlGKDeviceArguments.GKDeviceParameter);
					}
					break;

				case ProcedureStepType.ControlSKDDevice:
					{
						var controlSKDDeviceArguments = step.ControlSKDDeviceArguments;
						InvalidateArgument(procedure, controlSKDDeviceArguments.SKDDeviceParameter);
					}
					break;

				case ProcedureStepType.ControlGKFireZone:
					{
						var controlGKFireZoneArguments = step.ControlGKFireZoneArguments;
						InvalidateArgument(procedure, controlGKFireZoneArguments.GKFireZoneParameter);
					}
					break;

				case ProcedureStepType.ControlGKGuardZone:
					{
						var controlGKGuardZoneArguments = step.ControlGKGuardZoneArguments;
						InvalidateArgument(procedure, controlGKGuardZoneArguments.GKGuardZoneParameter);
					}
					break;

				case ProcedureStepType.ControlDirection:
					{
						var controlDirectionArguments = step.ControlDirectionArguments;
						InvalidateArgument(procedure, controlDirectionArguments.DirectionParameter);
					}
					break;

				case ProcedureStepType.ControlDoor:
					{
						var controlDoorArguments = step.ControlDoorArguments;
						InvalidateArgument(procedure, controlDoorArguments.DoorParameter);
					}
					break;

				case ProcedureStepType.ControlSKDZone:
					{
						var controlSKDZoneArguments = step.ControlSKDZoneArguments;
						InvalidateArgument(procedure, controlSKDZoneArguments.SKDZoneParameter);
					}
					break;

				case ProcedureStepType.ControlCamera:
					{
						var controlCameraArguments = step.ControlCameraArguments;
						InvalidateArgument(procedure, controlCameraArguments.CameraParameter);
					}
					break;

				case ProcedureStepType.GetObjectProperty:
					{
						var getObjectPropertyArguments = step.GetObjectPropertyArguments;
						InvalidateArgument(procedure, getObjectPropertyArguments.ObjectParameter);
						InvalidateArgument(procedure, getObjectPropertyArguments.ResultParameter);
					}
					break;

				case ProcedureStepType.SendEmail:
					{
						var sendEmailArguments = step.SendEmailArguments;
						InvalidateArgument(procedure, sendEmailArguments.EMailAddressParameter);
						InvalidateArgument(procedure, sendEmailArguments.EMailContentParameter);
						InvalidateArgument(procedure, sendEmailArguments.EMailTitleParameter);
						InvalidateArgument(procedure, sendEmailArguments.HostParameter);
						InvalidateArgument(procedure, sendEmailArguments.LoginParameter);
						InvalidateArgument(procedure, sendEmailArguments.PasswordParameter);
						InvalidateArgument(procedure, sendEmailArguments.PortParameter);
					}
					break;

				case ProcedureStepType.RunProgramm:
					{
						var runProgrammArguments = step.RunProgrammArguments;
						InvalidateArgument(procedure, runProgrammArguments.ParametersParameter);
						InvalidateArgument(procedure, runProgrammArguments.PathParameter);
					}
					break;

				case ProcedureStepType.Random:
					{
						var randomArguments = step.RandomArguments;
						InvalidateArgument(procedure, randomArguments.MaxValueParameter);
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
	}
}