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
					var soundStep = (SoundStep)step;
					if (ClientManager.SystemConfiguration.AutomationConfiguration.AutomationSounds.All(x => x.Uid != soundStep.SoundUid))
						Errors.Add(new ProcedureStepValidationError(step, "Необходимо выбрать звук", ValidationErrorLevel.CannotSave));
					break;

				case ProcedureStepType.ShowMessage:
					var showMessageStep = (ShowMessageStep)step;
					ValidateArgument(procedure, step, showMessageStep.MessageArgument);
					if (showMessageStep.WithConfirmation)
						ValidateArgument(procedure, step, showMessageStep.ConfirmationValueArgument);
					break;

				case ProcedureStepType.Arithmetics:
					var arithmeticStep = (ArithmeticStep)step;
					if (!ValidateArgument(procedure, step, arithmeticStep.Argument1))
						break;
					if (!ValidateArgument(procedure, step, arithmeticStep.Argument2))
						break;
					ValidateArgument(procedure, step, arithmeticStep.ResultArgument);
					break;

				case ProcedureStepType.CreateColor:
					var createColorStep = (CreateColorStep)step;
					if (!ValidateArgument(procedure, step, createColorStep.AArgument))
						break;
					if (!ValidateArgument(procedure, step, createColorStep.RArgument))
						break;
					if (!ValidateArgument(procedure, step, createColorStep.GArgument))
						break;
					if (!ValidateArgument(procedure, step, createColorStep.BArgument))
						break;
					ValidateArgument(procedure, step, createColorStep.ResultArgument);
					break;

				case ProcedureStepType.If:
				case ProcedureStepType.While:
					var conditionStep = (ConditionStep)step;
					foreach (var condition in conditionStep.Conditions)
					{
						if (!ValidateArgument(procedure, step, condition.Argument1))
							break;
						ValidateArgument(procedure, step, condition.Argument2);
					}
					break;

				case ProcedureStepType.AddJournalItem:
					var journalStep = (JournalStep)step;
					ValidateArgument(procedure, step, journalStep.MessageArgument);
					break;

				case ProcedureStepType.FindObjects:
					var findObjectStep = (FindObjectStep)step;
					if (!ValidateArgument(procedure, step, findObjectStep.ResultArgument))
						break;
					foreach (var findObjectCondition in findObjectStep.FindObjectConditions)
					{
						if (!ValidateArgument(procedure, step, findObjectCondition.SourceArgument))
							break;
					}
					break;

				case ProcedureStepType.Foreach:
					var foreachStep = (ForeachStep)step;
					if (!ValidateArgument(procedure, step, foreachStep.ItemArgument))
						break;
					ValidateArgument(procedure, step, foreachStep.ListArgument);
					break;

				case ProcedureStepType.For:
					var forStep = (ForStep)step;
					if (!ValidateArgument(procedure, step, forStep.IndexerArgument))
						break;
					if (!ValidateArgument(procedure, step, forStep.InitialValueArgument))
						break;
					if (!ValidateArgument(procedure, step, forStep.ValueArgument))
						break;
					ValidateArgument(procedure, step, forStep.IteratorArgument);
					break;

				case ProcedureStepType.Pause:
					var pauseStep = (PauseStep)step;
					ValidateArgument(procedure, step, pauseStep.PauseArgument);
					break;

				case ProcedureStepType.ProcedureSelection:
					var procedureSelectionStep = (ProcedureSelectionStep)step;
					if (ClientManager.SystemConfiguration.AutomationConfiguration.Procedures.All(x => x.Uid != procedureSelectionStep.ScheduleProcedure.ProcedureUid))
						Errors.Add(new ProcedureStepValidationError(step, "Необходимо выбрать процедуру", ValidationErrorLevel.CannotSave));
					foreach (var argument in procedureSelectionStep.ScheduleProcedure.Arguments)
						ValidateArgument(procedure, step, argument);
					break;

				case ProcedureStepType.SetValue:
					var setValueStep = (SetValueStep)step;
					if (!ValidateArgument(procedure, step, setValueStep.SourceArgument))
						break;
					ValidateArgument(procedure, step, setValueStep.TargetArgument);
					break;

				case ProcedureStepType.IncrementValue:
					var incrementValueStep = (IncrementValueStep)step;
					ValidateArgument(procedure, step, incrementValueStep.ResultArgument);
					break;

				case ProcedureStepType.ControlGKDevice:
					if (!GlobalSettingsHelper.GlobalSettings.IgnoredErrors.Contains(ValidationErrorType.DeviceHaveSelfLogik))
					{
						var controlGKDeviceStep = (ControlGKDeviceStep)step;
						var gkDevice = GKManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == controlGKDeviceStep.GKDeviceArgument.ExplicitValue.ObjectReferenceValue.UID);
						if (gkDevice != null && gkDevice.Logic.OnClausesGroup.Clauses.Count > 0)
							Errors.Add(new ProcedureStepValidationError(step, "Исполнительное устройство содержит собственную логику", ValidationErrorLevel.Warning));
						ValidateArgument(procedure, step, controlGKDeviceStep.GKDeviceArgument);
					}
					break;

				case ProcedureStepType.ControlGKFireZone:
					if (!LicenseManager.CurrentLicenseInfo.HasFirefighting)
						Errors.Add(new ProcedureStepValidationError(step, "Отсутствует лицензия для выполнения шага", ValidationErrorLevel.Warning));
					var controlGKFireZoneStep = (ControlGKFireZoneStep)step;
					ValidateArgument(procedure, step, controlGKFireZoneStep.GKFireZoneArgument);
					break;

				case ProcedureStepType.ControlGKGuardZone:
					if (!LicenseManager.CurrentLicenseInfo.HasGuard)
						Errors.Add(new ProcedureStepValidationError(step, "Отсутствует лицензия для выполнения шага", ValidationErrorLevel.Warning));
					var controlGKGuardZoneStep = (ControlGKGuardZoneStep)step;
					ValidateArgument(procedure, step, controlGKGuardZoneStep.GKGuardZoneArgument);
					break;

				case ProcedureStepType.ControlDirection:
					var controlDirectionStep = (ControlDirectionStep)step;
					ValidateArgument(procedure, step, controlDirectionStep.DirectionArgument);
					break;

				case ProcedureStepType.ControlGKDoor:
					if (!LicenseManager.CurrentLicenseInfo.HasSKD)
						Errors.Add(new ProcedureStepValidationError(step, "Отсутствует лицензия для выполнения шага", ValidationErrorLevel.Warning));
					var controlGKDoorStep = (ControlGKDoorStep)step;
					ValidateArgument(procedure, step, controlGKDoorStep.DoorArgument);
					break;

				case ProcedureStepType.ControlDelay:
					var controlDelayStep = (ControlDelayStep)step;
					ValidateArgument(procedure, step, controlDelayStep.DelayArgument);
					break;

				case ProcedureStepType.GetObjectProperty:
					var getObjectPropertyStep = (GetObjectPropertyStep)step;
					if (!ValidateArgument(procedure, step, getObjectPropertyStep.ObjectArgument))
						break;
					ValidateArgument(procedure, step, getObjectPropertyStep.ResultArgument);
					break;

				case ProcedureStepType.SendEmail:
					var sendEmailStep = (SendEmailStep)step;
					if (!ValidateArgument(procedure, step, sendEmailStep.EMailAddressFromArgument))
						break;
					if (!ValidateArgument(procedure, step, sendEmailStep.EMailAddressToArgument))
						break;
					if (!ValidateArgument(procedure, step, sendEmailStep.EMailContentArgument))
						break;
					if (!ValidateArgument(procedure, step, sendEmailStep.EMailTitleArgument))
						break;
					if (!ValidateArgument(procedure, step, sendEmailStep.SmtpArgument))
						break;
					if (!ValidateArgument(procedure, step, sendEmailStep.LoginArgument))
						break;
					if (!ValidateArgument(procedure, step, sendEmailStep.PasswordArgument))
						break;
					ValidateArgument(procedure, step, sendEmailStep.PortArgument);
					break;

				case ProcedureStepType.RunProgram:
					var RunProgramStep = (RunProgramStep)step;
					if (!ValidateArgument(procedure, step, RunProgramStep.ParametersArgument))
						break;
					ValidateArgument(procedure, step, RunProgramStep.PathArgument);
					break;

				case ProcedureStepType.Random:
					var randomStep = (RandomStep)step;
					ValidateArgument(procedure, step, randomStep.MaxValueArgument);
					break;

				case ProcedureStepType.ChangeList:
					var changeListStep = (ChangeListStep)step;
					if (!ValidateArgument(procedure, step, changeListStep.ItemArgument))
						break;
					ValidateArgument(procedure, step, changeListStep.ListArgument);
					break;

				case ProcedureStepType.CheckPermission:
					var checkPermissionStep = (CheckPermissionStep)step;
					if (!ValidateArgument(procedure, step, checkPermissionStep.PermissionArgument))
						break;
					ValidateArgument(procedure, step, checkPermissionStep.ResultArgument);
					break;

				case ProcedureStepType.GetJournalItem:
					var getJournalItemStep = (GetJournalItemStep)step;
					ValidateArgument(procedure, step, getJournalItemStep.ResultArgument);
					break;

				case ProcedureStepType.GetListCount:
					var getListCountStep = (GetListCountStep)step;
					if (!ValidateArgument(procedure, step, getListCountStep.ListArgument))
						break;
					ValidateArgument(procedure, step, getListCountStep.CountArgument);
					break;

				case ProcedureStepType.GetListItem:
					var getListItemStep = (GetListItemStep)step;
					if (!ValidateArgument(procedure, step, getListItemStep.ListArgument))
						break;
					if (!ValidateArgument(procedure, step, getListItemStep.ItemArgument))
						break;
					if (getListItemStep.PositionType == PositionType.ByIndex)
						ValidateArgument(procedure, step, getListItemStep.IndexArgument);
					break;

				case ProcedureStepType.ControlVisualGet:
				case ProcedureStepType.ControlVisualSet:
					var controlVisualStep = (ControlVisualStep)step;
					if (!ValidateArgument(procedure, step, controlVisualStep.Argument))
						break;
					if (controlVisualStep.Layout == Guid.Empty)
						Errors.Add(new ProcedureStepValidationError(step, "Не выбран макет", ValidationErrorLevel.CannotSave));
					else if (controlVisualStep.LayoutPart == Guid.Empty)
						Errors.Add(new ProcedureStepValidationError(step, "Не выбран элемент макета", ValidationErrorLevel.CannotSave));
					else if (!controlVisualStep.Property.HasValue)
						Errors.Add(new ProcedureStepValidationError(step, "Не выбрано свойство", ValidationErrorLevel.CannotSave));
					break;

				case ProcedureStepType.ControlPlanGet:
				case ProcedureStepType.ControlPlanSet:
					var controlPlanStep = (ControlPlanStep)step;
					ValidateArgument(procedure, step, controlPlanStep.ValueArgument);
					if (controlPlanStep.PlanUid == Guid.Empty)
						Errors.Add(new ProcedureStepValidationError(step, "Не выбран план", ValidationErrorLevel.CannotSave));
					else if (controlPlanStep.ElementUid == Guid.Empty)
						Errors.Add(new ProcedureStepValidationError(step, "Не выбран элемент плана", ValidationErrorLevel.CannotSave));
					break;

				case ProcedureStepType.ControlOpcDaTagGet:
				case ProcedureStepType.ControlOpcDaTagSet:
					var controlOpcDaTagStep = (ControlOpcDaTagStep)step;
					if (controlOpcDaTagStep.OpcDaServerUID == Guid.Empty)
						Errors.Add(new ProcedureStepValidationError(step, "Не выбран OPC DA Сервер", ValidationErrorLevel.CannotSave));
					else if (controlOpcDaTagStep.OpcDaTagUID == Guid.Empty)
						Errors.Add(new ProcedureStepValidationError(step, "Не выбран OPC DA Тэг", ValidationErrorLevel.CannotSave));
					else
						ValidateArgument(procedure, step, controlOpcDaTagStep.ValueArgument);
					break;

				case ProcedureStepType.ShowDialog:
					var showDialogStep = (ShowDialogStep)step;
					if (showDialogStep.Layout == Guid.Empty)
						Errors.Add(new ProcedureStepValidationError(step, "Не выбран макет", ValidationErrorLevel.CannotSave));
					break;

				case ProcedureStepType.CloseDialog:
					var closeDialogStep = (CloseDialogStep)step;
					ValidateArgument(procedure, step, closeDialogStep.WindowIDArgument);
					break;

				case ProcedureStepType.GenerateGuid:
					var generateGuidStep = (GenerateGuidStep)step;
					ValidateArgument(procedure, step, generateGuidStep.ResultArgument);
					break;

				case ProcedureStepType.SetJournalItemGuid:
					var setJournalItemGuidStep = (SetJournalItemGuidStep)step;
					ValidateArgument(procedure, step, setJournalItemGuidStep.ValueArgument);
					break;

				case ProcedureStepType.ExportJournal:
					var exportJournalStep = (ExportJournalStep)step;
					if (!ValidateArgument(procedure, step, exportJournalStep.IsExportJournalArgument))
						break;
					if (!ValidateArgument(procedure, step, exportJournalStep.IsExportPassJournalArgument))
						break;
					if (!ValidateArgument(procedure, step, exportJournalStep.MaxDateArgument))
						break;
					if (!ValidateArgument(procedure, step, exportJournalStep.MinDateArgument))
						break;
					ValidateArgument(procedure, step, exportJournalStep.PathArgument);
					break;

				case ProcedureStepType.ExportConfiguration:
					var exportConfigurationStep = (ExportConfigurationStep)step;
					if (!ValidateArgument(procedure, step, exportConfigurationStep.IsExportDevices))
						break;
					if (!ValidateArgument(procedure, step, exportConfigurationStep.IsExportDoors))
						break;
					if (!ValidateArgument(procedure, step, exportConfigurationStep.IsExportZones))
						break;
					ValidateArgument(procedure, step, exportConfigurationStep.PathArgument);
					break;

				case ProcedureStepType.ExportOrganisation:
					var exportOrganisationStep = (ExportOrganisationStep)step;
					if (!ValidateArgument(procedure, step, exportOrganisationStep.IsWithDeleted))
						break;
					if (!ValidateArgument(procedure, step, exportOrganisationStep.Organisation))
						break;
					ValidateArgument(procedure, step, exportOrganisationStep.PathArgument);
					break;

				case ProcedureStepType.ExportOrganisationList:
					var exportOrganisationListStep = (ExportOrganisationListStep)step;
					if (!ValidateArgument(procedure, step, exportOrganisationListStep.IsWithDeleted))
						break;
					ValidateArgument(procedure, step, exportOrganisationListStep.PathArgument);
					break;

				case ProcedureStepType.ImportOrganisationList:
					var importOrganisationListStep = (ImportOrganisationListStep)step;
					if (!ValidateArgument(procedure, step, importOrganisationListStep.IsWithDeleted))
						break;
					ValidateArgument(procedure, step, importOrganisationListStep.PathArgument);
					break;

				case ProcedureStepType.ImportOrganisation:
					var importOrganisationStep = (ImportOrganisationStep)step;
					if (!ValidateArgument(procedure, step, importOrganisationStep.IsWithDeleted))
						break;
					ValidateArgument(procedure, step, importOrganisationStep.PathArgument);
					break;

				case ProcedureStepType.Ptz:
					if (!LicenseManager.CurrentLicenseInfo.HasVideo)
						Errors.Add(new ProcedureStepValidationError(step, "Отсутствует лицензия для выполнения шага", ValidationErrorLevel.Warning));
					var ptzStep = (PtzStep)step;
					if (!ValidateArgument(procedure, step, ptzStep.CameraArgument))
						break;
					ValidateArgument(procedure, step, ptzStep.PtzNumberArgument);
					break;

				case ProcedureStepType.StartRecord:
					if (!LicenseManager.CurrentLicenseInfo.HasVideo)
						Errors.Add(new ProcedureStepValidationError(step, "Отсутствует лицензия для выполнения шага", ValidationErrorLevel.Warning));
					var startRecordStep = (StartRecordStep)step;
					if (!ValidateArgument(procedure, step, startRecordStep.CameraArgument))
						break;
					if (!ValidateArgument(procedure, step, startRecordStep.EventUIDArgument))
						break;
					ValidateArgument(procedure, step, startRecordStep.TimeoutArgument);
					break;

				case ProcedureStepType.StopRecord:
					if (!LicenseManager.CurrentLicenseInfo.HasVideo)
						Errors.Add(new ProcedureStepValidationError(step, "Отсутствует лицензия для выполнения шага", ValidationErrorLevel.Warning));
					var stopRecordStep = (StopRecordStep)step;
					if (!ValidateArgument(procedure, step, stopRecordStep.CameraArgument))
						break;
					ValidateArgument(procedure, step, stopRecordStep.EventUIDArgument);
					break;

				case ProcedureStepType.RviAlarm:
					if (!LicenseManager.CurrentLicenseInfo.HasVideo)
						Errors.Add(new ProcedureStepValidationError(step, "Отсутствует лицензия для выполнения шага", ValidationErrorLevel.Warning));
					var rviAlarmStep = (RviAlarmStep)step;
					ValidateArgument(procedure, step, rviAlarmStep.NameArgument);
					break;

				case ProcedureStepType.RviOpenWindow:
					if (!LicenseManager.CurrentLicenseInfo.HasVideo)
						Errors.Add(new ProcedureStepValidationError(step, "Отсутствует лицензия для выполнения шага", ValidationErrorLevel.Warning));
					var rviOpenWindowStep = (RviOpenWindowStep)step;
					ValidateArgument(procedure, step, rviOpenWindowStep.NameArgument);
					break;

				case ProcedureStepType.Now:
					var nowStep = (NowStep)step;
					ValidateArgument(procedure, step, nowStep.ResultArgument);
					break;

				case ProcedureStepType.HttpRequest:
					var httpRequestStep = (HttpRequestStep)step;
					ValidateArgument(procedure, step, httpRequestStep.UrlArgument);
					if (httpRequestStep.HttpMethod == HttpMethod.Post)
						ValidateArgument(procedure, step, httpRequestStep.ContentArgument);
					ValidateArgument(procedure, step, httpRequestStep.ResponseArgument);
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