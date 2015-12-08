using Infrastructure.Automation;
using RubezhAPI;
using RubezhAPI.Automation;
using RubezhAPI.AutomationCallback;
using RubezhAPI.GK;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FiresecService.Service
{
	public partial class FiresecService
	{
		public OperationResult<bool> RunProcedure(Guid clientUID, Guid procedureUID, List<Argument> args)
		{
			var procedure = ProcedureExecutionContext.SystemConfiguration.AutomationConfiguration.Procedures.FirstOrDefault(x => x.Uid == procedureUID);
			if (procedure != null)
			{
				var user = ProcedureExecutionContext.SecurityConfiguration.Users.FirstOrDefault(x => x.Login == CurrentClientCredentials.UserName);
				AutomationProcessor.RunProcedure(procedure, args, null, user, null, clientUID);
				return new OperationResult<bool>(true);
			}
			return OperationResult<bool>.FromError("Процедура не найдена");
		}

		public void ProcedureCallbackResponse(Guid callbackUID, object value, Guid clientUID)
		{
			ProcedureThread.CallbackResponse(callbackUID, value);
		}

		public ProcedureProperties GetProperties(Guid layoutUID, Guid clientUID)
		{
			return ProcedurePropertyCache.GetProcedureProperties(layoutUID);
		}

		public void SetVariableValue(Guid variableUid, object value, Guid clientUID)
		{
			var variable = GetVariable(variableUid, clientUID);
			if (variable != null)
				ProcedureExecutionContext.SetVariableValue(variable, value, clientUID);
		}

		public Variable GetVariable(Guid variableUid, Guid clientUID)
		{
			return ProcedureExecutionContext.SystemConfiguration.AutomationConfiguration.GlobalVariables.FirstOrDefault(x => x.Uid == variableUid);
		}


		public void AddJournalItemA(string message, Guid clientUID)
		{
			ProcedureHelper.AddJournalItem(message, clientUID);
		}

		public void ControlGKDevice(Guid deviceUid, GKStateBit command, Guid clientUID)
		{
			ProcedureHelper.ControlGKDevice(deviceUid, command, clientUID);
		}

		public void StartRecord(Guid cameraUid, Guid? journalItemUid, Guid? eventUid, int timeout, Guid clientUID)
		{
			ProcedureHelper.StartRecord(cameraUid, journalItemUid, eventUid, timeout, clientUID);
		}

		public void StopRecord(Guid cameraUid, Guid eventUid, Guid clientUID)
		{
			ProcedureHelper.StopRecord(cameraUid, eventUid, clientUID);
		}

		public void Ptz(Guid cameraUid, int ptzNumber, Guid clientUID)
		{
			ProcedureHelper.Ptz(cameraUid, ptzNumber, clientUID);
		}

		public void RviAlarm(string name, Guid clientUID)
		{
			ProcedureHelper.RviAlarm(name, clientUID);
		}

		public void ControlFireZone(Guid uid, ZoneCommandType commandType, Guid clientUID)
		{
			ProcedureHelper.ControlFireZone(uid, commandType, clientUID);
		}

		public void ControlGuardZone(Guid uid, GuardZoneCommandType commandType, Guid clientUID)
		{
			ProcedureHelper.ControlGuardZone(uid, commandType, clientUID);
		}

		public void ControlDirection(Guid uid, DirectionCommandType commandType, Guid clientUID)
		{
			ProcedureHelper.ControlDirection(uid, commandType, clientUID);
		}

		public void ControlGKDoor(Guid uid, GKDoorCommandType commandType, Guid clientUID)
		{
			ProcedureHelper.ControlGKDoor(uid, commandType, clientUID);
		}

		public void ControlDelay(Guid uid, DelayCommandType commandType, Guid clientUID)
		{
			ProcedureHelper.ControlDelay(uid, commandType, clientUID);
		}

		public void ControlPumpStation(Guid uid, PumpStationCommandType commandType, Guid clientUID)
		{
			ProcedureHelper.ControlPumpStation(uid, commandType, clientUID);
		}

		public void ControlMPT(Guid uid, MPTCommandType commandType, Guid clientUID)
		{
			ProcedureHelper.ControlMPT(uid, commandType, clientUID);
		}

		public void ExportJournalA(bool isExportJournal, bool isExportPassJournal, DateTime minDate, DateTime maxDate, string path, Guid clientUID)
		{
			ProcedureHelper.ExportJournal(isExportJournal, isExportPassJournal, minDate, maxDate, path, clientUID);
		}

		public void ExportOrganisationA(bool isWithDeleted, Guid organisationUid, string path, Guid clientUID)
		{
			ProcedureHelper.ExportOrganisation(isWithDeleted, organisationUid, path, clientUID);
		}

		public void ExportOrganisationListA(bool isWithDeleted, string path, Guid clientUID)
		{
			ProcedureHelper.ExportOrganisationList(isWithDeleted, path, clientUID);
		}

		public void ExportConfigurationA(bool isExportDevices, bool isExportDoors, bool isExportZones, string path, Guid clientUID)
		{
			ProcedureHelper.ExportConfiguration(isExportDevices, isExportDoors, isExportZones, path, clientUID);
		}

		public void ImportOrganisationA(bool isWithDeleted, string path, Guid clientUID)
		{
			ProcedureHelper.ImportOrganisation(isWithDeleted, path, clientUID);
		}

		public void ImportOrganisationListA(bool isWithDeleted, string path, Guid clientUID)
		{
			ProcedureHelper.ImportOrganisationList(isWithDeleted, path, clientUID);
		}
	}
}