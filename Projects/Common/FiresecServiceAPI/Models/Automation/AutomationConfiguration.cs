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
						InvalidateArithmeticParameter(procedure, showMessageArguments.MessageParameter);
					}
					break;

				case ProcedureStepType.Arithmetics:
					{
						var arithmeticArguments = step.ArithmeticArguments;
						InvalidateArithmeticParameter(procedure, arithmeticArguments.Parameter1);
						InvalidateArithmeticParameter(procedure, arithmeticArguments.Parameter2);
						InvalidateArithmeticParameter(procedure, arithmeticArguments.ResultParameter);
					}
					break;

				case ProcedureStepType.If:
					{
						var conditionArguments = step.ConditionArguments;
						foreach (var condition in conditionArguments.Conditions)
						{
							InvalidateArithmeticParameter(procedure, condition.Parameter1);
							InvalidateArithmeticParameter(procedure, condition.Parameter2);
						}
					}
					break;

				case ProcedureStepType.AddJournalItem:
					{
						var journalArguments = step.JournalArguments;
						InvalidateArithmeticParameter(procedure, journalArguments.MessageParameter);
					}
					break;

				case ProcedureStepType.FindObjects:
					{
						var findObjectArguments = step.FindObjectArguments;
						InvalidateArithmeticParameter(procedure, findObjectArguments.ResultParameter);
						foreach (var findObjectCondition in findObjectArguments.FindObjectConditions)
						{
							InvalidateArithmeticParameter(procedure, findObjectCondition.SourceParameter);
						}
					}
					break;

				case ProcedureStepType.Foreach:
					{
						var foreachArguments = step.ForeachArguments;
						InvalidateArithmeticParameter(procedure, foreachArguments.ItemParameter);
						InvalidateArithmeticParameter(procedure, foreachArguments.ListParameter);
					}
					break;

				case ProcedureStepType.Pause:
					{
						var pauseArguments = step.PauseArguments;
						InvalidateArithmeticParameter(procedure, pauseArguments.PauseParameter);
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
						InvalidateArithmeticParameter(procedure, exitArguments.ExitCodeParameter);
					}
					break;

				case ProcedureStepType.SetValue:
					{
						var setValueArguments = step.SetValueArguments;
						InvalidateArithmeticParameter(procedure, setValueArguments.SourceParameter);
						InvalidateArithmeticParameter(procedure, setValueArguments.TargetParameter);
					}
					break;

				case ProcedureStepType.IncrementValue:
					{
						var incrementValueArguments = step.IncrementValueArguments;
						InvalidateArithmeticParameter(procedure, incrementValueArguments.ResultParameter);
					}
					break;

				case ProcedureStepType.ControlGKDevice:
					{
						var controlGKDeviceArguments = step.ControlGKDeviceArguments;
						InvalidateArithmeticParameter(procedure, controlGKDeviceArguments.GKDeviceParameter);
					}
					break;

				case ProcedureStepType.ControlSKDDevice:
					{
						var controlSKDDeviceArguments = step.ControlSKDDeviceArguments;
						InvalidateArithmeticParameter(procedure, controlSKDDeviceArguments.SKDDeviceParameter);
					}
					break;

				case ProcedureStepType.ControlGKFireZone:
					{
						var controlGKFireZoneArguments = step.ControlGKFireZoneArguments;
						InvalidateArithmeticParameter(procedure, controlGKFireZoneArguments.GKFireZoneParameter);
					}
					break;

				case ProcedureStepType.ControlGKGuardZone:
					{
						var controlGKGuardZoneArguments = step.ControlGKGuardZoneArguments;
						InvalidateArithmeticParameter(procedure, controlGKGuardZoneArguments.GKGuardZoneParameter);
					}
					break;

				case ProcedureStepType.ControlDirection:
					{
						var controlDirectionArguments = step.ControlDirectionArguments;
						InvalidateArithmeticParameter(procedure, controlDirectionArguments.DirectionParameter);
					}
					break;

				case ProcedureStepType.ControlDoor:
					{
						var controlDoorArguments = step.ControlDoorArguments;
						InvalidateArithmeticParameter(procedure, controlDoorArguments.DoorParameter);
					}
					break;

				case ProcedureStepType.ControlSKDZone:
					{
						var controlSKDZoneArguments = step.ControlSKDZoneArguments;
						InvalidateArithmeticParameter(procedure, controlSKDZoneArguments.SKDZoneParameter);
					}
					break;

				case ProcedureStepType.ControlCamera:
					{
						var controlCameraArguments = step.ControlCameraArguments;
						InvalidateArithmeticParameter(procedure, controlCameraArguments.CameraParameter);
					}
					break;

				case ProcedureStepType.GetObjectProperty:
					{
						var getObjectPropertyArguments = step.GetObjectPropertyArguments;
						InvalidateArithmeticParameter(procedure, getObjectPropertyArguments.ObjectParameter);
						InvalidateArithmeticParameter(procedure, getObjectPropertyArguments.ResultParameter);
					}
					break;

				case ProcedureStepType.SendEmail:
					{
						var sendEmailArguments = step.SendEmailArguments;
						InvalidateArithmeticParameter(procedure, sendEmailArguments.EMailAddressParameter);
						InvalidateArithmeticParameter(procedure, sendEmailArguments.EMailContentParameter);
						InvalidateArithmeticParameter(procedure, sendEmailArguments.EMailTitleParameter);
						InvalidateArithmeticParameter(procedure, sendEmailArguments.HostParameter);
						InvalidateArithmeticParameter(procedure, sendEmailArguments.LoginParameter);
						InvalidateArithmeticParameter(procedure, sendEmailArguments.PasswordParameter);
						InvalidateArithmeticParameter(procedure, sendEmailArguments.PortParameter);
					}
					break;

				case ProcedureStepType.RunProgramm:
					{
						var runProgrammArguments = step.RunProgrammArguments;
						InvalidateArithmeticParameter(procedure, runProgrammArguments.ParametersParameter);
						InvalidateArithmeticParameter(procedure, runProgrammArguments.PathParameter);
					}
					break;

				case ProcedureStepType.Random:
					{
						var randomArguments = step.RandomArguments;
						InvalidateArithmeticParameter(procedure, randomArguments.MaxValueParameter);
					}
					break;
			}
		}

		void InvalidateArithmeticParameter(Procedure procedure, ArithmeticParameter arithmeticParameter)
		{
			var localVariables = new List<Variable>(procedure.Variables);
			localVariables.AddRange(new List<Variable>(procedure.Arguments));
			if (arithmeticParameter.VariableScope == VariableScope.GlobalVariable)
				if (GlobalVariables.All(x => x.Uid != arithmeticParameter.VariableUid))
					arithmeticParameter.VariableUid = Guid.Empty;
			if (arithmeticParameter.VariableScope == VariableScope.LocalVariable)
				if (localVariables.All(x => x.Uid != arithmeticParameter.VariableUid))
					arithmeticParameter.VariableUid = Guid.Empty;
		}
	}
}