﻿using Infrastructure.Common;
using Infrastructure.Common.Validation;
using RubezhAPI;
using RubezhAPI.Automation;
using RubezhClient;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AutomationModule.Validation
{
	public partial class Validator
	{
		Procedure Procedure { get; set; }

		void ValidateProcedure()
		{
			var nameList = new List<string>();
			foreach (var procedure in ClientManager.SystemConfiguration.AutomationConfiguration.Procedures)
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
						if (ClientManager.SystemConfiguration.AutomationConfiguration.AutomationSounds.All(x => x.Uid != soundArguments.SoundUid))
							Errors.Add(new ProcedureStepValidationError(step, "Все переменные должны быть инициализированы" + step.Name, ValidationErrorLevel.CannotSave));
					}
					break;

				case ProcedureStepType.ShowMessage:
					{
						var showMessageArguments = step.ShowMessageArguments;
						ValidateArgument(step, showMessageArguments.MessageArgument);
						if (showMessageArguments.WithConfirmation)
							ValidateArgument(step, showMessageArguments.ConfirmationValueArgument);
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
						if (ClientManager.SystemConfiguration.AutomationConfiguration.Procedures.All(x => x.Uid != procedureSelectionArguments.ScheduleProcedure.ProcedureUid))
							Errors.Add(new ProcedureStepValidationError(step, "Все переменные должны быть инициализированы" + step.Name, ValidationErrorLevel.CannotSave));
						foreach (var argument in procedureSelectionArguments.ScheduleProcedure.Arguments)
							ValidateArgument(step, argument);
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
						if (!GlobalSettingsHelper.GlobalSettings.IgnoredErrors.Contains(ValidationErrorType.DeviceHaveSelfLogik))
						{
							var controlGKDeviceArguments = step.ControlGKDeviceArguments;
							var gkDevice = GKManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == controlGKDeviceArguments.GKDeviceArgument.ExplicitValue.UidValue);
							if (gkDevice != null && gkDevice.Logic.OnClausesGroup.Clauses.Count > 0)
								Errors.Add(new ProcedureStepValidationError(step, "Исполнительное устройство содержит собственную логику" + step.Name, ValidationErrorLevel.Warning));
							ValidateArgument(step, controlGKDeviceArguments.GKDeviceArgument);
						}
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

				case ProcedureStepType.ControlGKDoor:
					{
						var controlGKDoorArguments = step.ControlGKDoorArguments;
						ValidateArgument(step, controlGKDoorArguments.DoorArgument);
					}
					break;

				case ProcedureStepType.ControlDelay:
					{
						var controlDelayArguments = step.ControlDelayArguments;
						ValidateArgument(step, controlDelayArguments.DelayArgument);
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
						if (!ValidateArgument(step, sendEmailArguments.EMailAddressFromArgument))
							break;
						if (!ValidateArgument(step, sendEmailArguments.EMailAddressToArgument))
							break;
						if (!ValidateArgument(step, sendEmailArguments.EMailContentArgument))
							break;
						if (!ValidateArgument(step, sendEmailArguments.EMailTitleArgument))
							break;
						if (!ValidateArgument(step, sendEmailArguments.SmtpArgument))
							break;
						if (!ValidateArgument(step, sendEmailArguments.LoginArgument))
							break;
						if (!ValidateArgument(step, sendEmailArguments.PasswordArgument))
							break;
						ValidateArgument(step, sendEmailArguments.PortArgument);
					}
					break;

				case ProcedureStepType.RunProgram:
					{
						var RunProgramArguments = step.RunProgramArguments;
						if (!ValidateArgument(step, RunProgramArguments.ParametersArgument))
							break;
						ValidateArgument(step, RunProgramArguments.PathArgument);
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
				case ProcedureStepType.ControlPlanGet:
				case ProcedureStepType.ControlPlanSet:
					var controlPlanArguments = step.ControlPlanArguments;
					ValidateArgument(step, controlPlanArguments.ValueArgument);
					if (controlPlanArguments.PlanUid == Guid.Empty)
						Errors.Add(new ProcedureStepValidationError(step, "Не выбран план", ValidationErrorLevel.CannotSave));
					else if (controlPlanArguments.ElementUid == Guid.Empty)
						Errors.Add(new ProcedureStepValidationError(step, "Не выбран элемент плана", ValidationErrorLevel.CannotSave));
					break;
				case ProcedureStepType.ShowDialog:
					if (step.ShowDialogArguments.Layout == Guid.Empty)
						Errors.Add(new ProcedureStepValidationError(step, "Не выбран макет", ValidationErrorLevel.CannotSave));
					break;
				case ProcedureStepType.GenerateGuid:
					{
						var generateGuidArguments = step.GenerateGuidArguments;
						ValidateArgument(step, generateGuidArguments.ResultArgument);
					}
					break;
				case ProcedureStepType.SetJournalItemGuid:
					{
						var setJournalItemGuidArguments = step.SetJournalItemGuidArguments;
						ValidateArgument(step, setJournalItemGuidArguments.ValueArgument);
					}
					break;
				case ProcedureStepType.ExportJournal:
					{
						var arguments = step.ExportJournalArguments;
						if (!ValidateArgument(step, arguments.IsExportJournalArgument))
							break;
						if (!ValidateArgument(step, arguments.IsExportPassJournalArgument))
							break;
						if (!ValidateArgument(step, arguments.MaxDateArgument))
							break;
						if (!ValidateArgument(step, arguments.MinDateArgument))
							break;
						ValidateArgument(step, arguments.PathArgument);
						break;
					}
				case ProcedureStepType.ExportConfiguration:
					{
						var arguments = step.ExportConfigurationArguments;
						if (!ValidateArgument(step, arguments.IsExportDevices))
							break;
						if (!ValidateArgument(step, arguments.IsExportDoors))
							break;
						if (!ValidateArgument(step, arguments.IsExportZones))
							break;
						ValidateArgument(step, arguments.PathArgument);
						break;
					}
				case ProcedureStepType.ExportOrganisation:
					{
						var arguments = step.ExportOrganisationArguments;
						if (!ValidateArgument(step, arguments.IsWithDeleted))
							break;
						if (!ValidateArgument(step, arguments.Organisation))
							break;
						ValidateArgument(step, arguments.PathArgument);
						break;
					}
				case ProcedureStepType.ExportOrganisationList:
					{
						var arguments = step.ImportOrganisationArguments;
						if (!ValidateArgument(step, arguments.IsWithDeleted))
							break;
						ValidateArgument(step, arguments.PathArgument);
						break;
					}
				case ProcedureStepType.ImportOrganisationList:
					{
						var arguments = step.ImportOrganisationArguments;
						if (!ValidateArgument(step, arguments.IsWithDeleted))
							break;
						ValidateArgument(step, arguments.PathArgument);
						break;
					}
				case ProcedureStepType.ImportOrganisation:
					{
						var arguments = step.ImportOrganisationArguments;
						if (!ValidateArgument(step, arguments.IsWithDeleted))
							break;
						ValidateArgument(step, arguments.PathArgument);
						break;
					}

				case ProcedureStepType.Ptz:
					{
						var arguments = step.PtzArguments;
						if (!ValidateArgument(step, arguments.CameraArgument))
							break;
						ValidateArgument(step, arguments.PtzNumberArgument);
						break;
					}
				case ProcedureStepType.StartRecord:
					{
						var arguments = step.StartRecordArguments;
						if (!ValidateArgument(step, arguments.CameraArgument))
							break;
						if (!ValidateArgument(step, arguments.EventUIDArgument))
							break;
						ValidateArgument(step, arguments.TimeoutArgument);
						break;
					}
				case ProcedureStepType.StopRecord:
					{
						var arguments = step.StopRecordArguments;
						if (!ValidateArgument(step, arguments.CameraArgument))
							break;
						ValidateArgument(step, arguments.EventUIDArgument);
						break;
					}
				case ProcedureStepType.RviAlarm:
					{
						var arguments = step.RviAlarmArguments;
						ValidateArgument(step, arguments.NameArgument);
						break;
					}
				case ProcedureStepType.Now:
					{
						var nowArguments = step.NowArguments;
						ValidateArgument(step, nowArguments.ResultArgument);
					}
					break;
				case ProcedureStepType.ControlOpcDaTagGet:
				case ProcedureStepType.ControlOpcDaTagSet:
					var controlOpcDaTagArguments = step.ControlOpcDaTagArguments;
					ValidateArgument(step, controlOpcDaTagArguments.ValueArgument);
					if (controlOpcDaTagArguments.OpcDaServerUID == Guid.Empty)
						Errors.Add(new ProcedureStepValidationError(step, "Не выбран OPC DA Сервер", ValidationErrorLevel.CannotSave));
					else if (controlOpcDaTagArguments.OpcDaTagUID == Guid.Empty)
						Errors.Add(new ProcedureStepValidationError(step, "Не выбран тэг", ValidationErrorLevel.CannotSave));
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
				if (ClientManager.SystemConfiguration.AutomationConfiguration.GlobalVariables.All(x => x.Uid != argument.VariableUid))
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