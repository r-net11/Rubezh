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
			var procedure = ConfigurationCashHelper.SystemConfiguration.AutomationConfiguration.Procedures.FirstOrDefault(x => x.Uid == procedureUID);
			if (procedure != null)
			{
				var user = ConfigurationCashHelper.SecurityConfiguration.Users.FirstOrDefault(x => x.Login == GetLogin(clientUID));
				AutomationProcessor.RunProcedure(procedure, args, null, user, null, clientUID);
				return new OperationResult<bool>(true);
			}
			return OperationResult<bool>.FromError("Процедура не найдена");
		}

		public void ProcedureCallbackResponse(Guid clientUID, Guid callbackUID, object value)
		{
			ProcedureThread.CallbackResponse(callbackUID, value);
		}

		public ProcedureProperties GetProperties(Guid clientUID, Guid layoutUID)
		{
			return ProcedurePropertyCache.GetProcedureProperties(layoutUID);
		}

		public void SetVariableValue(Guid clientUID, Guid variableUid, object value)
		{
			var variable = ProcedureExecutionContext.GlobalVariables.FirstOrDefault(x => x.Uid == variableUid);
			if (variable != null)
				ProcedureExecutionContext.SetVariableValue(variable, value, clientUID);
		}

		public void AddJournalItemA(Guid clientUID, string message, Guid? objectUID = null)
		{
			ProcedureHelper.AddJournalItem(clientUID, message, objectUID);
		}

		public void ControlGKDevice(Guid clientUID, Guid deviceUid, GKStateBit command)
		{
			ProcedureHelper.ControlGKDevice(clientUID, deviceUid, command);
		}

		public void StartRecord(Guid clientUID, Guid cameraUid, Guid? journalItemUid, Guid? eventUid, int timeout)
		{
			ProcedureHelper.StartRecord(clientUID, cameraUid, journalItemUid, eventUid, timeout);
		}

		public void StopRecord(Guid clientUID, Guid cameraUid, Guid eventUid)
		{
			ProcedureHelper.StopRecord(clientUID, cameraUid, eventUid);
		}

		public void Ptz(Guid clientUID, Guid cameraUid, int ptzNumber)
		{
			ProcedureHelper.Ptz(clientUID, cameraUid, ptzNumber);
		}

		public void RviAlarm(Guid clientUID, string name)
		{
			ProcedureHelper.RviAlarm(clientUID, name);
		}

		public void RviOpenWindow(Guid clientUid, string name, int x, int y, int monitorNumber, string login, string ip)
		{
			ProcedureHelper.RviOpenWindow(clientUid, name, x, y, monitorNumber, login, ip);
		}

		public void ControlFireZone(Guid clientUID, Guid uid, ZoneCommandType commandType)
		{
			ProcedureHelper.ControlFireZone(clientUID, uid, commandType);
		}

		public void ControlGuardZone(Guid clientUID, Guid uid, GuardZoneCommandType commandType)
		{
			ProcedureHelper.ControlGuardZone(clientUID, uid, commandType);
		}

		public void ControlDirection(Guid clientUID, Guid uid, DirectionCommandType commandType)
		{
			ProcedureHelper.ControlDirection(clientUID, uid, commandType);
		}

		public void ControlGKDoor(Guid clientUID, Guid uid, GKDoorCommandType commandType)
		{
			ProcedureHelper.ControlGKDoor(clientUID, uid, commandType);
		}

		public void ControlDelay(Guid clientUID, Guid uid, DelayCommandType commandType)
		{
			ProcedureHelper.ControlDelay(clientUID, uid, commandType);
		}

		public void ControlPumpStation(Guid clientUID, Guid uid, PumpStationCommandType commandType)
		{
			ProcedureHelper.ControlPumpStation(clientUID, uid, commandType);
		}

		public void ControlMPT(Guid clientUID, Guid uid, MPTCommandType commandType)
		{
			ProcedureHelper.ControlMPT(clientUID, uid, commandType);
		}

		public void ExportJournalA(Guid clientUID, bool isExportJournal, bool isExportPassJournal, DateTime minDate, DateTime maxDate, string path)
		{
			ProcedureHelper.ExportJournal(clientUID, isExportJournal, isExportPassJournal, minDate, maxDate, path);
		}

		public void ExportOrganisationA(Guid clientUID, bool isWithDeleted, Guid organisationUid, string path)
		{
			ProcedureHelper.ExportOrganisation(clientUID, isWithDeleted, organisationUid, path);
		}

		public void ExportOrganisationListA(Guid clientUID, bool isWithDeleted, string path)
		{
			ProcedureHelper.ExportOrganisationList(clientUID, isWithDeleted, path);
		}

		public void ExportConfigurationA(Guid clientUID, bool isExportDevices, bool isExportDoors, bool isExportZones, string path)
		{
			ProcedureHelper.ExportConfiguration(clientUID, isExportDevices, isExportDoors, isExportZones, path);
		}

		public void ImportOrganisationA(Guid clientUID, bool isWithDeleted, string path)
		{
			ProcedureHelper.ImportOrganisation(clientUID, isWithDeleted, path);
		}

		public void ImportOrganisationListA(Guid clientUID, bool isWithDeleted, string path)
		{
			ProcedureHelper.ImportOrganisationList(clientUID, isWithDeleted, path);
		}
	}
}