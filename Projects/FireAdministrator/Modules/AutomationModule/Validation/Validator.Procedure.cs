using Infrastructure.Common;
using Infrastructure.Common.Validation;
using RubezhAPI;
using RubezhAPI.Automation;
using RubezhAPI.License;
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
					ValidateStep(procedure, procedureStep);

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

		void ValidateStep(Procedure procedure, ProcedureStep step)
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
						ValidateArgument(procedure, step, showMessageArguments.MessageArgument);
						if (showMessageArguments.WithConfirmation)
							ValidateArgument(procedure, step, showMessageArguments.ConfirmationValueArgument);
					}
					break;

				case ProcedureStepType.Arithmetics:
					{
						var arithmeticArguments = step.ArithmeticArguments;
						if (!ValidateArgument(procedure, step, arithmeticArguments.Argument1))
							break;
						if (!ValidateArgument(procedure, step, arithmeticArguments.Argument2))
							break;
						ValidateArgument(procedure, step, arithmeticArguments.ResultArgument);
					}
					break;

				case ProcedureStepType.If:
				case ProcedureStepType.While:
					{
						var conditionArguments = step.ConditionArguments;
						foreach (var condition in conditionArguments.Conditions)
						{
							if (!ValidateArgument(procedure, step, condition.Argument1))
								break;
							ValidateArgument(procedure, step, condition.Argument2);
						}
					}
					break;

				case ProcedureStepType.AddJournalItem:
					{
						var journalArguments = step.JournalArguments;
						ValidateArgument(procedure, step, journalArguments.MessageArgument);
					}
					break;

				case ProcedureStepType.FindObjects:
					{
						var findObjectArguments = step.FindObjectArguments;
						if (!ValidateArgument(procedure, step, findObjectArguments.ResultArgument))
							break;
						foreach (var findObjectCondition in findObjectArguments.FindObjectConditions)
						{
							if (!ValidateArgument(procedure, step, findObjectCondition.SourceArgument))
								break;
						}
					}
					break;

				case ProcedureStepType.Foreach:
					{
						var foreachArguments = step.ForeachArguments;
						if (!ValidateArgument(procedure, step, foreachArguments.ItemArgument))
							break;
						ValidateArgument(procedure, step, foreachArguments.ListArgument);
					}
					break;

				case ProcedureStepType.For:
					{
						var forArguments = step.ForArguments;
						if (!ValidateArgument(procedure, step, forArguments.IndexerArgument))
							break;
						if (!ValidateArgument(procedure, step, forArguments.InitialValueArgument))
							break;
						if (!ValidateArgument(procedure, step, forArguments.ValueArgument))
							break;
						ValidateArgument(procedure, step, forArguments.IteratorArgument);
					}
					break;

				case ProcedureStepType.Pause:
					{
						var pauseArguments = step.PauseArguments;
						ValidateArgument(procedure, step, pauseArguments.PauseArgument);
					}
					break;

				case ProcedureStepType.ProcedureSelection:
					{
						var procedureSelectionArguments = step.ProcedureSelectionArguments;
						if (ClientManager.SystemConfiguration.AutomationConfiguration.Procedures.All(x => x.Uid != procedureSelectionArguments.ScheduleProcedure.ProcedureUid))
							Errors.Add(new ProcedureStepValidationError(step, "Все переменные должны быть инициализированы" + step.Name, ValidationErrorLevel.CannotSave));
						foreach (var argument in procedureSelectionArguments.ScheduleProcedure.Arguments)
							ValidateArgument(procedure, step, argument);
					}
					break;

				case ProcedureStepType.SetValue:
					{
						var setValueArguments = step.SetValueArguments;
						if (!ValidateArgument(procedure, step, setValueArguments.SourceArgument))
							break;
						ValidateArgument(procedure, step, setValueArguments.TargetArgument);
					}
					break;

				case ProcedureStepType.IncrementValue:
					{
						var incrementValueArguments = step.IncrementValueArguments;
						ValidateArgument(procedure, step, incrementValueArguments.ResultArgument);
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
							ValidateArgument(procedure, step, controlGKDeviceArguments.GKDeviceArgument);
						}
					}
					break;

				case ProcedureStepType.ControlGKFireZone:
					{
						if (!LicenseManager.CurrentLicenseInfo.HasFirefighting)
							Errors.Add(new ProcedureStepValidationError(step, "Отсутствует лицензия для выполнения шага " + step.Name, ValidationErrorLevel.Warning));
						var controlGKFireZoneArguments = step.ControlGKFireZoneArguments;
						ValidateArgument(procedure, step, controlGKFireZoneArguments.GKFireZoneArgument);
					}
					break;

				case ProcedureStepType.ControlGKGuardZone:
					{
						if (!LicenseManager.CurrentLicenseInfo.HasGuard)
							Errors.Add(new ProcedureStepValidationError(step, "Отсутствует лицензия для выполнения шага " + step.Name, ValidationErrorLevel.Warning));
						var controlGKGuardZoneArguments = step.ControlGKGuardZoneArguments;
						ValidateArgument(procedure, step, controlGKGuardZoneArguments.GKGuardZoneArgument);
					}
					break;

				case ProcedureStepType.ControlDirection:
					{
						var controlDirectionArguments = step.ControlDirectionArguments;
						ValidateArgument(procedure, step, controlDirectionArguments.DirectionArgument);
					}
					break;

				case ProcedureStepType.ControlGKDoor:
					{
						if (!LicenseManager.CurrentLicenseInfo.HasSKD)
							Errors.Add(new ProcedureStepValidationError(step, "Отсутствует лицензия для выполнения шага " + step.Name, ValidationErrorLevel.Warning));
						var controlGKDoorArguments = step.ControlGKDoorArguments;
						ValidateArgument(procedure, step, controlGKDoorArguments.DoorArgument);
					}
					break;

				case ProcedureStepType.ControlDelay:
					{
						var controlDelayArguments = step.ControlDelayArguments;
						ValidateArgument(procedure, step, controlDelayArguments.DelayArgument);
					}
					break;

				case ProcedureStepType.GetObjectProperty:
					{
						var getObjectPropertyArguments = step.GetObjectPropertyArguments;
						if (!ValidateArgument(procedure, step, getObjectPropertyArguments.ObjectArgument))
							break;
						ValidateArgument(procedure, step, getObjectPropertyArguments.ResultArgument);
					}
					break;

				case ProcedureStepType.SendEmail:
					{
						var sendEmailArguments = step.SendEmailArguments;
						if (!ValidateArgument(procedure, step, sendEmailArguments.EMailAddressFromArgument))
							break;
						if (!ValidateArgument(procedure, step, sendEmailArguments.EMailAddressToArgument))
							break;
						if (!ValidateArgument(procedure, step, sendEmailArguments.EMailContentArgument))
							break;
						if (!ValidateArgument(procedure, step, sendEmailArguments.EMailTitleArgument))
							break;
						if (!ValidateArgument(procedure, step, sendEmailArguments.SmtpArgument))
							break;
						if (!ValidateArgument(procedure, step, sendEmailArguments.LoginArgument))
							break;
						if (!ValidateArgument(procedure, step, sendEmailArguments.PasswordArgument))
							break;
						ValidateArgument(procedure, step, sendEmailArguments.PortArgument);
					}
					break;

				case ProcedureStepType.RunProgram:
					{
						var RunProgramArguments = step.RunProgramArguments;
						if (!ValidateArgument(procedure, step, RunProgramArguments.ParametersArgument))
							break;
						ValidateArgument(procedure, step, RunProgramArguments.PathArgument);
					}
					break;

				case ProcedureStepType.Random:
					{
						var randomArguments = step.RandomArguments;
						ValidateArgument(procedure, step, randomArguments.MaxValueArgument);
					}
					break;
				case ProcedureStepType.ChangeList:
					{
						var changeListArguments = step.ChangeListArguments;
						if (!ValidateArgument(procedure, step, changeListArguments.ItemArgument))
							break;
						ValidateArgument(procedure, step, changeListArguments.ListArgument);
					}
					break;

				case ProcedureStepType.CheckPermission:
					{
						var checkPermissionArguments = step.CheckPermissionArguments;
						if (!ValidateArgument(procedure, step, checkPermissionArguments.PermissionArgument))
							break;
						ValidateArgument(procedure, step, checkPermissionArguments.ResultArgument);
					}
					break;

				case ProcedureStepType.GetJournalItem:
					{
						var getJournalItemArguments = step.GetJournalItemArguments;
						ValidateArgument(procedure, step, getJournalItemArguments.ResultArgument);
					}
					break;

				case ProcedureStepType.GetListCount:
					{
						var getListCountArgument = step.GetListCountArguments;
						if (!ValidateArgument(procedure, step, getListCountArgument.ListArgument))
							break;
						ValidateArgument(procedure, step, getListCountArgument.CountArgument);
					}
					break;

				case ProcedureStepType.GetListItem:
					{
						var getListItemArgument = step.GetListItemArguments;
						if (!ValidateArgument(procedure, step, getListItemArgument.ListArgument))
							break;
						if (!ValidateArgument(procedure, step, getListItemArgument.ItemArgument))
							break;
						if (getListItemArgument.PositionType == PositionType.ByIndex)
							ValidateArgument(procedure, step, getListItemArgument.IndexArgument);
					}
					break;
				case ProcedureStepType.ControlVisualGet:
				case ProcedureStepType.ControlVisualSet:
					var controlVisualArguments = step.ControlVisualArguments;
					if (!ValidateArgument(procedure, step, controlVisualArguments.Argument))
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
					ValidateArgument(procedure, step, controlPlanArguments.ValueArgument);
					if (controlPlanArguments.PlanUid == Guid.Empty)
						Errors.Add(new ProcedureStepValidationError(step, "Не выбран план", ValidationErrorLevel.CannotSave));
					else if (controlPlanArguments.ElementUid == Guid.Empty)
						Errors.Add(new ProcedureStepValidationError(step, "Не выбран элемент плана", ValidationErrorLevel.CannotSave));
					break;
				case ProcedureStepType.ControlOpcDaTagGet:
				case ProcedureStepType.ControlOpcDaTagSet:
					var controlOpcDaTagArguments = step.ControlOpcDaTagArguments;
					if (controlOpcDaTagArguments.OpcDaServerUID == Guid.Empty)
						Errors.Add(new ProcedureStepValidationError(step, "Не выбран OPC DA Сервер", ValidationErrorLevel.CannotSave));
					else if (controlOpcDaTagArguments.OpcDaTagUID == Guid.Empty)
						Errors.Add(new ProcedureStepValidationError(step, "Не выбран OPC DA Тэг", ValidationErrorLevel.CannotSave));
					else
						ValidateArgument(procedure, step, controlOpcDaTagArguments.ValueArgument);
					break;
				case ProcedureStepType.ShowDialog:
					if (step.ShowDialogArguments.Layout == Guid.Empty)
						Errors.Add(new ProcedureStepValidationError(step, "Не выбран макет", ValidationErrorLevel.CannotSave));
					break;
				case ProcedureStepType.CloseDialog:
					ValidateArgument(procedure, step, step.CloseDialogArguments.WindowIDArgument);
					break;
				case ProcedureStepType.GenerateGuid:
					{
						var generateGuidArguments = step.GenerateGuidArguments;
						ValidateArgument(procedure, step, generateGuidArguments.ResultArgument);
					}
					break;
				case ProcedureStepType.SetJournalItemGuid:
					{
						var setJournalItemGuidArguments = step.SetJournalItemGuidArguments;
						ValidateArgument(procedure, step, setJournalItemGuidArguments.ValueArgument);
					}
					break;
				case ProcedureStepType.ExportJournal:
					{
						var arguments = step.ExportJournalArguments;
						if (!ValidateArgument(procedure, step, arguments.IsExportJournalArgument))
							break;
						if (!ValidateArgument(procedure, step, arguments.IsExportPassJournalArgument))
							break;
						if (!ValidateArgument(procedure, step, arguments.MaxDateArgument))
							break;
						if (!ValidateArgument(procedure, step, arguments.MinDateArgument))
							break;
						ValidateArgument(procedure, step, arguments.PathArgument);
						break;
					}
				case ProcedureStepType.ExportConfiguration:
					{
						var arguments = step.ExportConfigurationArguments;
						if (!ValidateArgument(procedure, step, arguments.IsExportDevices))
							break;
						if (!ValidateArgument(procedure, step, arguments.IsExportDoors))
							break;
						if (!ValidateArgument(procedure, step, arguments.IsExportZones))
							break;
						ValidateArgument(procedure, step, arguments.PathArgument);
						break;
					}
				case ProcedureStepType.ExportOrganisation:
					{
						var arguments = step.ExportOrganisationArguments;
						if (!ValidateArgument(procedure, step, arguments.IsWithDeleted))
							break;
						if (!ValidateArgument(procedure, step, arguments.Organisation))
							break;
						ValidateArgument(procedure, step, arguments.PathArgument);
						break;
					}
				case ProcedureStepType.ExportOrganisationList:
					{
						var arguments = step.ImportOrganisationArguments;
						if (!ValidateArgument(procedure, step, arguments.IsWithDeleted))
							break;
						ValidateArgument(procedure, step, arguments.PathArgument);
						break;
					}
				case ProcedureStepType.ImportOrganisationList:
					{
						var arguments = step.ImportOrganisationArguments;
						if (!ValidateArgument(procedure, step, arguments.IsWithDeleted))
							break;
						ValidateArgument(procedure, step, arguments.PathArgument);
						break;
					}
				case ProcedureStepType.ImportOrganisation:
					{
						var arguments = step.ImportOrganisationArguments;
						if (!ValidateArgument(procedure, step, arguments.IsWithDeleted))
							break;
						ValidateArgument(procedure, step, arguments.PathArgument);
						break;
					}

				case ProcedureStepType.Ptz:
					{
						if (!LicenseManager.CurrentLicenseInfo.HasVideo)
							Errors.Add(new ProcedureStepValidationError(step, "Отсутствует лицензия для выполнения шага " + step.Name, ValidationErrorLevel.Warning));
						var arguments = step.PtzArguments;
						if (!ValidateArgument(procedure, step, arguments.CameraArgument))
							break;
						ValidateArgument(procedure, step, arguments.PtzNumberArgument);
						break;
					}
				case ProcedureStepType.StartRecord:
					{
						if (!LicenseManager.CurrentLicenseInfo.HasVideo)
							Errors.Add(new ProcedureStepValidationError(step, "Отсутствует лицензия для выполнения шага " + step.Name, ValidationErrorLevel.Warning));
						var arguments = step.StartRecordArguments;
						if (!ValidateArgument(procedure, step, arguments.CameraArgument))
							break;
						if (!ValidateArgument(procedure, step, arguments.EventUIDArgument))
							break;
						ValidateArgument(procedure, step, arguments.TimeoutArgument);
						break;
					}
				case ProcedureStepType.StopRecord:
					{
						if (!LicenseManager.CurrentLicenseInfo.HasVideo)
							Errors.Add(new ProcedureStepValidationError(step, "Отсутствует лицензия для выполнения шага " + step.Name, ValidationErrorLevel.Warning));
						var arguments = step.StopRecordArguments;
						if (!ValidateArgument(procedure, step, arguments.CameraArgument))
							break;
						ValidateArgument(procedure, step, arguments.EventUIDArgument);
						break;
					}
				case ProcedureStepType.RviAlarm:
					{
						if (!LicenseManager.CurrentLicenseInfo.HasVideo)
							Errors.Add(new ProcedureStepValidationError(step, "Отсутствует лицензия для выполнения шага " + step.Name, ValidationErrorLevel.Warning));
						var arguments = step.RviAlarmArguments;
						ValidateArgument(procedure, step, arguments.NameArgument);
						break;
					}
				case ProcedureStepType.RviOpenWindow:
					{
						if (!LicenseManager.CurrentLicenseInfo.HasVideo)
							Errors.Add(new ProcedureStepValidationError(step, "Отсутствует лицензия для выполнения шага " + step.Name, ValidationErrorLevel.Warning));
						var arguments = step.RviOpenWindowArguments;
						ValidateArgument(procedure, step, arguments.NameArgument);
						break;
					}
				case ProcedureStepType.Now:
					{
						var nowArguments = step.NowArguments;
						ValidateArgument(procedure, step, nowArguments.ResultArgument);
					}
					break;
				case ProcedureStepType.HttpRequest:
					var httpRequestArguments = step.HttpRequestArguments;
					ValidateArgument(procedure, step, httpRequestArguments.UrlArgument);
					if (httpRequestArguments.HttpMethod == HttpMethod.Post)
						ValidateArgument(procedure, step, httpRequestArguments.ContentArgument);
					ValidateArgument(procedure, step, httpRequestArguments.ResponseArgument);
					break;
			}
			foreach (var childStep in step.Children)
				ValidateStep(procedure, childStep);
		}

		bool ValidateArgument(Procedure procedure, ProcedureStep step, Argument argument)
		{
			var localVariables = new List<Variable>(Procedure.Variables);
			localVariables.AddRange(new List<Variable>(Procedure.Arguments));
			if (argument.VariableScope == VariableScope.GlobalVariable)
			{
				var variable = ClientManager.SystemConfiguration.AutomationConfiguration.GlobalVariables.FirstOrDefault(x => x.Uid == argument.VariableUid);
				if (variable == null)
				{
					Errors.Add(new ProcedureStepValidationError(step, "Все переменные должны быть инициализированы", ValidationErrorLevel.CannotSave));
					return false;
				}
				else if (variable.ContextType == ContextType.Client && procedure.ContextType == ContextType.Server)
				{
					Errors.Add(new ProcedureStepValidationError(step, "Глобальная переменная \"" + variable.Name + "\" будет определена на сервере, т.к. контекст исполнения процедуры - сервер", ValidationErrorLevel.Warning));
					return false;
				}
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