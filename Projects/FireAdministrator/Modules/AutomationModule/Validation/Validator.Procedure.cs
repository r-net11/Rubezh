using System.Collections.Generic;
using FiresecClient;
using Infrastructure.Common.Validation;
using FiresecAPI.Automation;
using AutomationModule.ViewModels;
using System;
using System.Linq;
using System.Reflection;
using Infrastructure.Common.Windows.ViewModels;

namespace AutomationModule.Validation
{
	public partial class Validator
	{
		Procedure Procedure { get; set; }

		void ValidateProcedureName()
		{
			var nameList = new List<string>();
			foreach (var procedure in FiresecManager.SystemConfiguration.AutomationConfiguration.Procedures)
			{
				Procedure = procedure;
				foreach (var procedureStep in procedure.Steps)
					ValidateStep(procedureStep);

				if (nameList.Contains(procedure.Name))
					Errors.Add(new ProcedureValidationError(procedure, "Процедура с таким именем уже существует " + procedure.Name, ValidationErrorLevel.CannotSave));
				nameList.Add(procedure.Name);
				
				var varList = new List<string>();
				foreach (var variable in procedure.Variables)
				{
					if (varList.Contains(variable.Name))
						Errors.Add(new VariableValidationError(variable, "Переменная с таким именем уже существует " + variable.Name, ValidationErrorLevel.CannotSave));
					varList.Add(variable.Name);
				}

				var argList = new List<string>();
				foreach (var argument in procedure.Arguments)
				{
					if (argList.Contains(argument.Name))
						Errors.Add(new VariableValidationError(argument, "Аргумент с таким именем уже существует " + argument.Name, ValidationErrorLevel.CannotSave));
					argList.Add(argument.Name);
				}
			}
		}

		void ValidateStep(ProcedureStep step)
		{
			switch (step.ProcedureStepType)
			{
				case ProcedureStepType.PlaySound:
					{
						var soundArguments = step.SoundArguments;
						if (FiresecManager.SystemConfiguration.AutomationConfiguration.AutomationSounds.All(x => x.Uid != soundArguments.SoundUid))
							Errors.Add(new ProcedureStepValidationError(step, "Все переменные должны быть инициализированы" + step.Name, ValidationErrorLevel.CannotSave));
					}
					break;

				case ProcedureStepType.ShowMessage:
					{
						var showMessageArguments = step.ShowMessageArguments;
						ValidateArithmeticParameter(step, showMessageArguments.MessageParameter);
					}
					break;

				case ProcedureStepType.Arithmetics:
					{
						var arithmeticArguments = step.ArithmeticArguments;
						if (!ValidateArithmeticParameter(step, arithmeticArguments.Parameter1))
							break;
						if (!ValidateArithmeticParameter(step, arithmeticArguments.Parameter2))
							break;
						ValidateArithmeticParameter(step, arithmeticArguments.ResultParameter);
					}
					break;

				case ProcedureStepType.If:
					{
						var conditionArguments = step.ConditionArguments;
						foreach (var condition in conditionArguments.Conditions)
						{
							if (!ValidateArithmeticParameter(step, condition.Parameter1))
								break;
							ValidateArithmeticParameter(step, condition.Parameter2);
						}
					}
					break;

				case ProcedureStepType.AddJournalItem:
					{
						var journalArguments = step.JournalArguments;
						ValidateArithmeticParameter(step, journalArguments.MessageParameter);
					}
					break;

				case ProcedureStepType.FindObjects:
					{
						var findObjectArguments = step.FindObjectArguments;
						if (!ValidateArithmeticParameter(step, findObjectArguments.ResultParameter))
							break;
						foreach (var findObjectCondition in findObjectArguments.FindObjectConditions)
						{
							if (!ValidateArithmeticParameter(step, findObjectCondition.SourceParameter))
								break;
						}
					}
					break;

				case ProcedureStepType.Foreach:
					{
						var foreachArguments = step.ForeachArguments;
						if (!ValidateArithmeticParameter(step, foreachArguments.ItemParameter))
							break;
						ValidateArithmeticParameter(step, foreachArguments.ListParameter);
					}
					break;

				case ProcedureStepType.Pause:
					{
						var pauseArguments = step.PauseArguments;
						ValidateArithmeticParameter(step, pauseArguments.PauseParameter);
					}
					break;

				case ProcedureStepType.ProcedureSelection:
					{
						var procedureSelectionArguments = step.ProcedureSelectionArguments;
						if (FiresecManager.SystemConfiguration.AutomationConfiguration.Procedures.All(x => x.Uid != procedureSelectionArguments.ScheduleProcedure.ProcedureUid))
							Errors.Add(new ProcedureStepValidationError(step, "Все переменные должны быть инициализированы" + step.Name, ValidationErrorLevel.CannotSave));
					}
					break;

				case ProcedureStepType.Exit:
					{
						var exitArguments = step.ExitArguments;
						ValidateArithmeticParameter(step, exitArguments.ExitCodeParameter);
					}
					break;

				case ProcedureStepType.SetValue:
					{
						var setValueArguments = step.SetValueArguments;
						if (!ValidateArithmeticParameter(step, setValueArguments.SourceParameter))
							break;
						ValidateArithmeticParameter(step, setValueArguments.TargetParameter);
					}
					break;

				case ProcedureStepType.IncrementValue:
					{
						var incrementValueArguments = step.IncrementValueArguments;
						ValidateArithmeticParameter(step, incrementValueArguments.ResultParameter);
					}
					break;

				case ProcedureStepType.ControlGKDevice:
					{
						var controlGKDeviceArguments = step.ControlGKDeviceArguments;
						ValidateArithmeticParameter(step, controlGKDeviceArguments.GKDeviceParameter);
					}
					break;

				case ProcedureStepType.ControlSKDDevice:
					{
						var controlSKDDeviceArguments = step.ControlSKDDeviceArguments;
						ValidateArithmeticParameter(step, controlSKDDeviceArguments.SKDDeviceParameter);
					}
					break;

				case ProcedureStepType.ControlGKFireZone:
					{
						var controlGKFireZoneArguments = step.ControlGKFireZoneArguments;
						ValidateArithmeticParameter(step, controlGKFireZoneArguments.GKFireZoneParameter);
					}
					break;

				case ProcedureStepType.ControlGKGuardZone:
					{
						var controlGKGuardZoneArguments = step.ControlGKGuardZoneArguments;
						ValidateArithmeticParameter(step, controlGKGuardZoneArguments.GKGuardZoneParameter);
					}
					break;

				case ProcedureStepType.ControlDirection:
					{
						var controlDirectionArguments = step.ControlDirectionArguments;
						ValidateArithmeticParameter(step, controlDirectionArguments.DirectionParameter);
					}
					break;

				case ProcedureStepType.ControlDoor:
					{
						var controlDoorArguments = step.ControlDoorArguments;
						ValidateArithmeticParameter(step, controlDoorArguments.DoorParameter);
					}
					break;

				case ProcedureStepType.ControlSKDZone:
					{
						var controlSKDZoneArguments = step.ControlSKDZoneArguments;
						ValidateArithmeticParameter(step, controlSKDZoneArguments.SKDZoneParameter);
					}
					break;

				case ProcedureStepType.ControlCamera:
					{
						var controlCameraArguments = step.ControlCameraArguments;
						ValidateArithmeticParameter(step, controlCameraArguments.CameraParameter);
					}
					break;

				case ProcedureStepType.GetObjectProperty:
					{
						var getObjectPropertyArguments = step.GetObjectPropertyArguments;
						if (!ValidateArithmeticParameter(step, getObjectPropertyArguments.ObjectParameter))
							break;
						ValidateArithmeticParameter(step, getObjectPropertyArguments.ResultParameter);
					}
					break;

				case ProcedureStepType.SendEmail:
					{
						var sendEmailArguments = step.SendEmailArguments;
						if (!ValidateArithmeticParameter(step, sendEmailArguments.EMailAddressParameter))
							break;
						if (!ValidateArithmeticParameter(step, sendEmailArguments.EMailContentParameter))
							break;
						if (!ValidateArithmeticParameter(step, sendEmailArguments.EMailTitleParameter))
							break;
						if (!ValidateArithmeticParameter(step, sendEmailArguments.HostParameter))
							break;
						if (!ValidateArithmeticParameter(step, sendEmailArguments.LoginParameter))
							break;
						if (!ValidateArithmeticParameter(step, sendEmailArguments.PasswordParameter))
							break;
						ValidateArithmeticParameter(step, sendEmailArguments.PortParameter);
					}
					break;

				case ProcedureStepType.RunProgramm:
					{
						var runProgrammArguments = step.RunProgrammArguments;
						if (!ValidateArithmeticParameter(step, runProgrammArguments.ParametersParameter))
							break;
						ValidateArithmeticParameter(step, runProgrammArguments.PathParameter);
					}
					break;

				case ProcedureStepType.Random:
					{
						var randomArguments = step.RandomArguments;
						ValidateArithmeticParameter(step, randomArguments.MaxValueParameter);
					}
					break;
			}
		}


		bool ValidateArithmeticParameter(ProcedureStep step, Variable arithmeticParameter)
		{
			var localVariables = new List<Variable>(Procedure.Variables);
			localVariables.AddRange(new List<Variable>(Procedure.Arguments));
			if (arithmeticParameter.VariableScope == VariableScope.GlobalVariable)
				if (FiresecManager.SystemConfiguration.AutomationConfiguration.GlobalVariables.All(x => x.Uid != arithmeticParameter.VariableUid))
				{
					Errors.Add(new ProcedureStepValidationError(step, "Все переменные должны быть инициализированы" + step.Name, ValidationErrorLevel.CannotSave));
					return false;
				}
			if (arithmeticParameter.VariableScope == VariableScope.LocalVariable)
				if (localVariables.All(x => x.Uid != arithmeticParameter.VariableUid))
				{
					Errors.Add(new ProcedureStepValidationError(step, "Все переменные должны быть инициализированы" + step.Name, ValidationErrorLevel.CannotSave));
					return false;
				}
			return true;
		}
	}
}