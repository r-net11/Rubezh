using System.Collections.Generic;
using FiresecClient;
using Infrastructure.Common.Validation;
using FiresecAPI.Automation;
using System.Linq;

namespace AutomationModule.Validation
{
	public partial class Validator
	{
		Procedure Procedure { get; set; }

		void ValidateProcedure()
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
						ValidateArgument(step, showMessageArguments.MessageParameter);
					}
					break;

				case ProcedureStepType.Arithmetics:
					{
						var arithmeticArguments = step.ArithmeticArguments;
						if (!ValidateArgument(step, arithmeticArguments.Parameter1))
							break;
						if (!ValidateArgument(step, arithmeticArguments.Parameter2))
							break;
						ValidateArgument(step, arithmeticArguments.ResultParameter);
					}
					break;

				case ProcedureStepType.If:
				case ProcedureStepType.While:
					{
						var conditionArguments = step.ConditionArguments;
						foreach (var condition in conditionArguments.Conditions)
						{
							if (!ValidateArgument(step, condition.Parameter1))
								break;
							ValidateArgument(step, condition.Parameter2);
						}
					}
					break;

				case ProcedureStepType.AddJournalItem:
					{
						var journalArguments = step.JournalArguments;
						ValidateArgument(step, journalArguments.MessageParameter);
					}
					break;

				case ProcedureStepType.FindObjects:
					{
						var findObjectArguments = step.FindObjectArguments;
						if (!ValidateArgument(step, findObjectArguments.ResultParameter))
							break;
						foreach (var findObjectCondition in findObjectArguments.FindObjectConditions)
						{
							if (!ValidateArgument(step, findObjectCondition.SourceParameter))
								break;
						}
					}
					break;

				case ProcedureStepType.Foreach:
					{
						var foreachArguments = step.ForeachArguments;
						if (!ValidateArgument(step, foreachArguments.ItemParameter))
							break;
						ValidateArgument(step, foreachArguments.ListParameter);
					}
					break;

				case ProcedureStepType.For:
					{
						var forArguments = step.ForArguments;
						if (!ValidateArgument(step, forArguments.IndexerArgument))
							break;
						if (!ValidateArgument(step, forArguments.InitialValueArgument))
							break;
						if (!ValidateArgument(step, forArguments.ValueArgument))
							break;
						ValidateArgument(step, forArguments.IteratorArgument);
					}
					break;

				case ProcedureStepType.Pause:
					{
						var pauseArguments = step.PauseArguments;
						ValidateArgument(step, pauseArguments.PauseParameter);
					}
					break;

				case ProcedureStepType.ProcedureSelection:
					{
						var procedureSelectionArguments = step.ProcedureSelectionArguments;
						if (FiresecManager.SystemConfiguration.AutomationConfiguration.Procedures.All(x => x.Uid != procedureSelectionArguments.ScheduleProcedure.ProcedureUid))
							Errors.Add(new ProcedureStepValidationError(step, "Все переменные должны быть инициализированы" + step.Name, ValidationErrorLevel.CannotSave));
						foreach(var argument in procedureSelectionArguments.ScheduleProcedure.Arguments)
							ValidateArgument(step, argument);
					}
					break;

				case ProcedureStepType.Exit:
					{
						var exitArguments = step.ExitArguments;
						ValidateArgument(step, exitArguments.ExitCodeParameter);
					}
					break;

				case ProcedureStepType.SetValue:
					{
						var setValueArguments = step.SetValueArguments;
						if (!ValidateArgument(step, setValueArguments.SourceParameter))
							break;
						ValidateArgument(step, setValueArguments.TargetParameter);
					}
					break;

				case ProcedureStepType.IncrementValue:
					{
						var incrementValueArguments = step.IncrementValueArguments;
						ValidateArgument(step, incrementValueArguments.ResultParameter);
					}
					break;

				case ProcedureStepType.ControlGKDevice:
					{
						var controlGKDeviceArguments = step.ControlGKDeviceArguments;
						ValidateArgument(step, controlGKDeviceArguments.GKDeviceParameter);
					}
					break;

				case ProcedureStepType.ControlSKDDevice:
					{
						var controlSKDDeviceArguments = step.ControlSKDDeviceArguments;
						ValidateArgument(step, controlSKDDeviceArguments.SKDDeviceParameter);
					}
					break;

				case ProcedureStepType.ControlGKFireZone:
					{
						var controlGKFireZoneArguments = step.ControlGKFireZoneArguments;
						ValidateArgument(step, controlGKFireZoneArguments.GKFireZoneParameter);
					}
					break;

				case ProcedureStepType.ControlGKGuardZone:
					{
						var controlGKGuardZoneArguments = step.ControlGKGuardZoneArguments;
						ValidateArgument(step, controlGKGuardZoneArguments.GKGuardZoneParameter);
					}
					break;

				case ProcedureStepType.ControlDirection:
					{
						var controlDirectionArguments = step.ControlDirectionArguments;
						ValidateArgument(step, controlDirectionArguments.DirectionParameter);
					}
					break;

				case ProcedureStepType.ControlDoor:
					{
						var controlDoorArguments = step.ControlDoorArguments;
						ValidateArgument(step, controlDoorArguments.DoorParameter);
					}
					break;

				case ProcedureStepType.ControlSKDZone:
					{
						var controlSKDZoneArguments = step.ControlSKDZoneArguments;
						ValidateArgument(step, controlSKDZoneArguments.SKDZoneParameter);
					}
					break;

				case ProcedureStepType.ControlCamera:
					{
						var controlCameraArguments = step.ControlCameraArguments;
						ValidateArgument(step, controlCameraArguments.CameraParameter);
					}
					break;

				case ProcedureStepType.GetObjectProperty:
					{
						var getObjectPropertyArguments = step.GetObjectPropertyArguments;
						if (!ValidateArgument(step, getObjectPropertyArguments.ObjectParameter))
							break;
						ValidateArgument(step, getObjectPropertyArguments.ResultParameter);
					}
					break;

				case ProcedureStepType.SendEmail:
					{
						var sendEmailArguments = step.SendEmailArguments;
						if (!ValidateArgument(step, sendEmailArguments.EMailAddressParameter))
							break;
						if (!ValidateArgument(step, sendEmailArguments.EMailContentParameter))
							break;
						if (!ValidateArgument(step, sendEmailArguments.EMailTitleParameter))
							break;
						if (!ValidateArgument(step, sendEmailArguments.HostParameter))
							break;
						if (!ValidateArgument(step, sendEmailArguments.LoginParameter))
							break;
						if (!ValidateArgument(step, sendEmailArguments.PasswordParameter))
							break;
						ValidateArgument(step, sendEmailArguments.PortParameter);
					}
					break;

				case ProcedureStepType.RunProgramm:
					{
						var runProgrammArguments = step.RunProgrammArguments;
						if (!ValidateArgument(step, runProgrammArguments.ParametersParameter))
							break;
						ValidateArgument(step, runProgrammArguments.PathParameter);
					}
					break;

				case ProcedureStepType.Random:
					{
						var randomArguments = step.RandomArguments;
						ValidateArgument(step, randomArguments.MaxValueParameter);
					}
					break;
				case ProcedureStepType.ChangeList:
					{
						var changeListArguments = step.ChangeListArguments;
						if (!ValidateArgument(step, changeListArguments.ItemArgument))
							break;
						ValidateArgument(step, changeListArguments.ListArgument);
					}
					break;

				case ProcedureStepType.GetListCount:
					{
						var getListCountArgument = step.GetListCountArgument;
						if (!ValidateArgument(step, getListCountArgument.ListArgument))
							break;
						ValidateArgument(step, getListCountArgument.CountArgument);
					}
					break;
			}
		}


		bool ValidateArgument(ProcedureStep step, Argument argument)
		{
			var localVariables = new List<Variable>(Procedure.Variables);
			localVariables.AddRange(new List<Variable>(Procedure.Arguments));
			if (argument.VariableScope == VariableScope.GlobalVariable)
				if (FiresecManager.SystemConfiguration.AutomationConfiguration.GlobalVariables.All(x => x.Uid != argument.VariableUid))
				{
					Errors.Add(new ProcedureStepValidationError(step, "Все переменные должны быть инициализированы" + step.Name, ValidationErrorLevel.CannotSave));
					return false;
				}
			if (argument.VariableScope == VariableScope.LocalVariable)
				if (localVariables.All(x => x.Uid != argument.VariableUid))
				{
					Errors.Add(new ProcedureStepValidationError(step, "Все переменные должны быть инициализированы" + step.Name, ValidationErrorLevel.CannotSave));
					return false;
				}
			return true;
		}
	}
}