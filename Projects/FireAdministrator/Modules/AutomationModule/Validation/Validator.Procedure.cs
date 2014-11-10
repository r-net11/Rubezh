using System.Collections.Generic;
using FiresecClient;
using Infrastructure.Common.Validation;
using FiresecAPI.Automation;
using System.Linq;
using System;

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
						ValidateArgument(step, showMessageArguments.MessageArgument);
					}
					break;

				case ProcedureStepType.Arithmetics:
					{
						var arithmeticArguments = step.ArithmeticArguments;
						if (!ValidateArgument(step, arithmeticArguments.Argument1))
							break;
						if (!ValidateArgument(step, arithmeticArguments.Argument2))
							break;
						ValidateArgument(step, arithmeticArguments.ResultArgument);
					}
					break;

				case ProcedureStepType.If:
				case ProcedureStepType.While:
					{
						var conditionArguments = step.ConditionArguments;
						foreach (var condition in conditionArguments.Conditions)
						{
							if (!ValidateArgument(step, condition.Argument1))
								break;
							ValidateArgument(step, condition.Argument2);
						}
					}
					break;

				case ProcedureStepType.AddJournalItem:
					{
						var journalArguments = step.JournalArguments;
						ValidateArgument(step, journalArguments.MessageArgument);
					}
					break;

				case ProcedureStepType.FindObjects:
					{
						var findObjectArguments = step.FindObjectArguments;
						if (!ValidateArgument(step, findObjectArguments.ResultArgument))
							break;
						foreach (var findObjectCondition in findObjectArguments.FindObjectConditions)
						{
							if (!ValidateArgument(step, findObjectCondition.SourceArgument))
								break;
						}
					}
					break;

				case ProcedureStepType.Foreach:
					{
						var foreachArguments = step.ForeachArguments;
						if (!ValidateArgument(step, foreachArguments.ItemArgument))
							break;
						ValidateArgument(step, foreachArguments.ListArgument);
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
						ValidateArgument(step, pauseArguments.PauseArgument);
					}
					break;

				case ProcedureStepType.ProcedureSelection:
					{
						var procedureSelectionArguments = step.ProcedureSelectionArguments;
						if (FiresecManager.SystemConfiguration.AutomationConfiguration.Procedures.All(x => x.Uid != procedureSelectionArguments.ScheduleProcedure.ProcedureUid))
							Errors.Add(new ProcedureStepValidationError(step, "Все переменные должны быть инициализированы" + step.Name, ValidationErrorLevel.CannotSave));
						foreach (var argument in procedureSelectionArguments.ScheduleProcedure.Arguments)
							ValidateArgument(step, argument);
					}
					break;

				case ProcedureStepType.Exit:
					{
						var exitArguments = step.ExitArguments;
						ValidateArgument(step, exitArguments.ExitCodeArgument);
					}
					break;

				case ProcedureStepType.SetValue:
					{
						var setValueArguments = step.SetValueArguments;
						if (!ValidateArgument(step, setValueArguments.SourceArgument))
							break;
						ValidateArgument(step, setValueArguments.TargetArgument);
					}
					break;

				case ProcedureStepType.IncrementValue:
					{
						var incrementValueArguments = step.IncrementValueArguments;
						ValidateArgument(step, incrementValueArguments.ResultArgument);
					}
					break;

				case ProcedureStepType.ControlGKDevice:
					{
						var controlGKDeviceArguments = step.ControlGKDeviceArguments;
						var gkDevice = GKManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == controlGKDeviceArguments.GKDeviceArgument.ExplicitValue.UidValue);
						if (gkDevice != null && gkDevice.DeviceLogic.ClausesGroup.Clauses.Count > 0)
							Errors.Add(new ProcedureStepValidationError(step, "Исполнительное устройство содержит собственную логику" + step.Name, ValidationErrorLevel.Warning));
						ValidateArgument(step, controlGKDeviceArguments.GKDeviceArgument);
					}
					break;

				case ProcedureStepType.ControlSKDDevice:
					{
						var controlSKDDeviceArguments = step.ControlSKDDeviceArguments;
						ValidateArgument(step, controlSKDDeviceArguments.SKDDeviceArgument);
					}
					break;

				case ProcedureStepType.ControlGKFireZone:
					{
						var controlGKFireZoneArguments = step.ControlGKFireZoneArguments;
						ValidateArgument(step, controlGKFireZoneArguments.GKFireZoneArgument);
					}
					break;

				case ProcedureStepType.ControlGKGuardZone:
					{
						var controlGKGuardZoneArguments = step.ControlGKGuardZoneArguments;
						ValidateArgument(step, controlGKGuardZoneArguments.GKGuardZoneArgument);
					}
					break;

				case ProcedureStepType.ControlDirection:
					{
						var controlDirectionArguments = step.ControlDirectionArguments;
						ValidateArgument(step, controlDirectionArguments.DirectionArgument);
					}
					break;

				case ProcedureStepType.ControlDoor:
					{
						var controlDoorArguments = step.ControlDoorArguments;
						ValidateArgument(step, controlDoorArguments.DoorArgument);
					}
					break;

				case ProcedureStepType.ControlSKDZone:
					{
						var controlSKDZoneArguments = step.ControlSKDZoneArguments;
						ValidateArgument(step, controlSKDZoneArguments.SKDZoneArgument);
					}
					break;

				case ProcedureStepType.ControlCamera:
					{
						var controlCameraArguments = step.ControlCameraArguments;
						ValidateArgument(step, controlCameraArguments.CameraArgument);
					}
					break;

				case ProcedureStepType.GetObjectProperty:
					{
						var getObjectPropertyArguments = step.GetObjectPropertyArguments;
						if (!ValidateArgument(step, getObjectPropertyArguments.ObjectArgument))
							break;
						ValidateArgument(step, getObjectPropertyArguments.ResultArgument);
					}
					break;

				case ProcedureStepType.SendEmail:
					{
						var sendEmailArguments = step.SendEmailArguments;
						if (!ValidateArgument(step, sendEmailArguments.EMailAddressArgument))
							break;
						if (!ValidateArgument(step, sendEmailArguments.EMailContentArgument))
							break;
						if (!ValidateArgument(step, sendEmailArguments.EMailTitleArgument))
							break;
						if (!ValidateArgument(step, sendEmailArguments.HostArgument))
							break;
						if (!ValidateArgument(step, sendEmailArguments.LoginArgument))
							break;
						if (!ValidateArgument(step, sendEmailArguments.PasswordArgument))
							break;
						ValidateArgument(step, sendEmailArguments.PortArgument);
					}
					break;

				case ProcedureStepType.RunProgramm:
					{
						var runProgrammArguments = step.RunProgrammArguments;
						if (!ValidateArgument(step, runProgrammArguments.ParametersArgument))
							break;
						ValidateArgument(step, runProgrammArguments.PathArgument);
					}
					break;

				case ProcedureStepType.Random:
					{
						var randomArguments = step.RandomArguments;
						ValidateArgument(step, randomArguments.MaxValueArgument);
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

				case ProcedureStepType.CheckPermission:
					{
						var checkPermissionArguments = step.CheckPermissionArguments;
						if (!ValidateArgument(step, checkPermissionArguments.PermissionArgument))
							break;
						ValidateArgument(step, checkPermissionArguments.ResultArgument);
					}
					break;

				case ProcedureStepType.GetJournalItem:
					{
						var getJournalItemArguments = step.GetJournalItemArguments;
						ValidateArgument(step, getJournalItemArguments.ResultArgument);
					}
					break;

				case ProcedureStepType.GetListCount:
					{
						var getListCountArgument = step.GetListCountArguments;
						if (!ValidateArgument(step, getListCountArgument.ListArgument))
							break;
						ValidateArgument(step, getListCountArgument.CountArgument);
					}
					break;

				case ProcedureStepType.GetListItem:
					{
						var getListItemArgument = step.GetListItemArguments;
						if (!ValidateArgument(step, getListItemArgument.ListArgument))
							break;
						if (!ValidateArgument(step, getListItemArgument.ItemArgument))
							break;
						if (getListItemArgument.PositionType == PositionType.ByIndex)
							ValidateArgument(step, getListItemArgument.IndexArgument);
					}
					break;
				case ProcedureStepType.ControlVisualGet:
				case ProcedureStepType.ControlVisualSet:
					var controlVisualArguments = step.ControlVisualArguments;
					if (!ValidateArgument(step, controlVisualArguments.Argument))
						break;
					if (controlVisualArguments.Layout == Guid.Empty)
						Errors.Add(new ProcedureStepValidationError(step, "Не выбран макет", ValidationErrorLevel.CannotSave));
					else if (controlVisualArguments.LayoutPart == Guid.Empty)
						Errors.Add(new ProcedureStepValidationError(step, "Не выбран элемент макета", ValidationErrorLevel.CannotSave));
					else if (!controlVisualArguments.Property.HasValue)
						Errors.Add(new ProcedureStepValidationError(step, "Не выбрано свойство", ValidationErrorLevel.CannotSave));
					break;
			}
			foreach (var childStep in step.Children)
				ValidateStep(childStep);
		}

		bool ValidateArgument(ProcedureStep step, Argument argument)
		{
			var localVariables = new List<Variable>(Procedure.Variables);
			localVariables.AddRange(new List<Variable>(Procedure.Arguments));
			if (argument.VariableScope == VariableScope.GlobalVariable)
				if (FiresecManager.SystemConfiguration.AutomationConfiguration.GlobalVariables.All(x => x.Uid != argument.VariableUid))
				{
					Errors.Add(new ProcedureStepValidationError(step, "Все переменные должны быть инициализированы", ValidationErrorLevel.CannotSave));
					return false;
				}
			if (argument.VariableScope == VariableScope.LocalVariable)
				if (localVariables.All(x => x.Uid != argument.VariableUid))
				{
					Errors.Add(new ProcedureStepValidationError(step, "Все переменные должны быть инициализированы", ValidationErrorLevel.CannotSave));
					return false;
				}
			return true;
		}
	}
}