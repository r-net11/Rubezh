using System;
using System.Collections.Generic;
using System.Linq;
using RubezhAPI;
using RubezhAPI.Automation;
using RubezhAPI.AutomationCallback;
using Infrastructure.Automation;
using RubezhAPI.GK;

namespace FiresecService.Service
{
	public partial class FiresecService : IFiresecService
	{
		public OperationResult<bool> RunProcedure(Guid clientUID, Guid procedureUID, List<Argument> args)
		{
			var procedure = ProcedureExecutionContext.SystemConfiguration.AutomationConfiguration.Procedures.FirstOrDefault(x => x.Uid == procedureUID);
			if (procedure != null)
			{
				var user = ProcedureExecutionContext.SecurityConfiguration.Users.FirstOrDefault(x => x.Login == CurrentClientCredentials.UserName);
				var result = AutomationProcessor.RunProcedure(procedure, args, null, user, null, clientUID);
				return new OperationResult<bool>(true);
			}
			return OperationResult<bool>.FromError("Процедура не найдена");
		}

		public void ProcedureCallbackResponse(Guid callbackUID, object value)
		{
			ProcedureThread.CallbackResponse(callbackUID, value);
		}

		public ProcedureProperties GetProperties(Guid layoutUID)
		{
			return ProcedurePropertyCache.GetProcedureProperties(layoutUID);
		}
		
		public void SetVariableValue(Guid variableUid, object value)
		{
			var variable = GetVariable(variableUid);
			if (variable != null)
				ProcedureExecutionContext.SetVariableValue(variable, value);
		}
		
		public Variable GetVariable(Guid variableUid)
		{
			return ProcedureExecutionContext.SystemConfiguration.AutomationConfiguration.GlobalVariables.FirstOrDefault(x => x.Uid == variableUid);
		}


		public void AddJournalItemA(string message)
		{
			ProcedureHelper.AddJournalItem(message);
		}

		public void ControlGKDevice(Guid deviceUid, GKStateBit command)
		{
			ProcedureHelper.ControlGKDevice(deviceUid, command);
		}

		public void StartRecord(Guid cameraUid, Guid? journalItemUid, Guid? eventUid, int timeout)
		{
			ProcedureHelper.StartRecord(cameraUid, journalItemUid, eventUid, timeout);
		}

		public void StopRecord(Guid cameraUid, Guid eventUid)
		{
			ProcedureHelper.StopRecord(cameraUid, eventUid);
		}

		public void Ptz(Guid cameraUid, int ptzNumber)
		{
			ProcedureHelper.Ptz(cameraUid, ptzNumber);
		}

		public void RviAlarm(string name)
		{
			ProcedureHelper.RviAlarm(name);
		}

		public void ControlFireZone(Guid uid, ZoneCommandType commandType)
		{
			ProcedureHelper.ControlFireZone(uid, commandType);
		}

		public void ControlGuardZone(Guid uid, GuardZoneCommandType commandType)
		{
			ProcedureHelper.ControlGuardZone(uid, commandType);
		}

		public void ControlDirection(Guid uid, DirectionCommandType commandType)
		{
			ProcedureHelper.ControlDirection(uid, commandType);
		}

		public void ControlGKDoor(Guid uid, GKDoorCommandType commandType)
		{
			ProcedureHelper.ControlGKDoor(uid, commandType);
		}

		public void ControlDelay(Guid uid, DelayCommandType commandType)
		{
			ProcedureHelper.ControlDelay(uid, commandType);
		}

		public void ExportJournalA(bool isExportJournal, bool isExportPassJournal, DateTime minDate, DateTime maxDate, string path)
		{
			ProcedureHelper.ExportJournal(isExportJournal, isExportPassJournal, minDate, maxDate, path);
		}

		public void ExportOrganisationA(bool isWithDeleted, Guid organisationUid, string path)
		{
			ProcedureHelper.ExportOrganisation(isWithDeleted, organisationUid, path);
		}

		public void ExportOrganisationListA(bool isWithDeleted, string path)
		{
			ProcedureHelper.ExportOrganisationList(isWithDeleted, path);
		}

		public void ExportConfigurationA(bool isExportDevices, bool isExportDoors, bool isExportZones, string path)
		{
			ProcedureHelper.ExportConfiguration(isExportDevices, isExportDoors, isExportZones, path);
		}

		public void ImportOrganisationA(bool isWithDeleted, string path)
		{
			ProcedureHelper.ImportOrganisation(isWithDeleted, path);
		}

		public void ImportOrganisationListA(bool isWithDeleted, string path)
		{
			ProcedureHelper.ImportOrganisationList(isWithDeleted, path);
		}
	}
}