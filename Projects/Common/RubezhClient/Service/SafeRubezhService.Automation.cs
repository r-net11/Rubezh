using RubezhAPI;
using RubezhAPI.Automation;
using RubezhAPI.AutomationCallback;
using RubezhAPI.GK;
using System;
using System.Collections.Generic;

namespace RubezhClient
{
	public partial class SafeRubezhService
	{
		public OperationResult<bool> RunProcedure(Guid clientUID, Guid procedureUID, List<Argument> args)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.RunProcedure(clientUID, procedureUID, args);
			}, "RunProcedure");
		}
		public void ProcedureCallbackResponse(Guid clientUID, Guid procedureThreadUID, object value)
		{
			SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					rubezhService.ProcedureCallbackResponse(clientUID, procedureThreadUID, value);
			}, "ProcedureCallbackResponse");
		}
		public ProcedureProperties GetProperties(Guid clientUID, Guid layoutUID)
		{
			return SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					return rubezhService.GetProperties(clientUID, layoutUID);
			}, "GetProperties");
		}

		public void SetVariableValue(Guid clientUID, Guid variableUid, object value)
		{
			SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					rubezhService.SetVariableValue(clientUID, variableUid, value);
			}, "SetVariableValue");
		}

		public void AddJournalItem(Guid clientUID, string message, Guid? objectUID = null)
		{
			SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					rubezhService.AddJournalItemA(clientUID, message, objectUID);
			}, "AddJournalItem");
		}

		public void ControlGKDevice(Guid clientUID, Guid deviceUid, GKStateBit command)
		{
			SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					rubezhService.ControlGKDevice(clientUID, deviceUid, command);
			}, "ControlGKDevice");
		}

		public void StartRecord(Guid clientUID, Guid cameraUid, Guid? journalItemUid, Guid? eventUid, int timeout)
		{
			SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					rubezhService.StartRecord(clientUID, cameraUid, journalItemUid, eventUid, timeout);
			}, "StartRecord");
		}

		public void StopRecord(Guid clientUID, Guid cameraUid, Guid eventUid)
		{
			SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					rubezhService.StopRecord(clientUID, cameraUid, eventUid);
			}, "StopRecord");
		}

		public void Ptz(Guid clientUID, Guid cameraUid, int ptzNumber)
		{
			SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					rubezhService.Ptz(clientUID, cameraUid, ptzNumber);
			}, "Ptz");
		}

		public void RviAlarm(Guid clientUID, string name)
		{
			SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					rubezhService.RviAlarm(clientUID, name);
			}, "RviAlarm");
		}

		public void RviOpenWindow(Guid clientUid, string name, int x, int y, int monitorNumber, string login, string ip)
		{
			SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					rubezhService.RviOpenWindow(clientUid, name, x, y, monitorNumber, login, ip);
			}, "RviOpenWindow");
		}

		public void ControlFireZone(Guid clientUID, Guid uid, ZoneCommandType commandType)
		{
			SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					rubezhService.ControlFireZone(clientUID, uid, commandType);
			}, "ControlFireZone");
		}

		public void ControlGuardZone(Guid clientUID, Guid uid, GuardZoneCommandType commandType)
		{
			SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					rubezhService.ControlGuardZone(clientUID, uid, commandType);
			}, "ControlGuardZone");
		}

		public void ControlDirection(Guid clientUID, Guid uid, DirectionCommandType commandType)
		{
			SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					rubezhService.ControlDirection(clientUID, uid, commandType);
			}, "ControlDirection");
		}

		public void ControlGKDoor(Guid clientUID, Guid uid, GKDoorCommandType commandType)
		{
			SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					rubezhService.ControlGKDoor(clientUID, uid, commandType);
			}, "ControlGKDoor");
		}

		public void ControlDelay(Guid clientUID, Guid uid, DelayCommandType commandType)
		{
			SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					rubezhService.ControlDelay(clientUID, uid, commandType);
			}, "ControlDelay");
		}

		public void ControlPumpStation(Guid clientUID, Guid uid, PumpStationCommandType commandType)
		{
			SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					rubezhService.ControlPumpStation(clientUID, uid, commandType);
			}, "ControlPumpStation");
		}

		public void ControlMPT(Guid clientUID, Guid uid, MPTCommandType commandType)
		{
			SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					rubezhService.ControlMPT(clientUID, uid, commandType);
			}, "ControlMPT");
		}

		public void ExportJournal(Guid clientUID, bool isExportJournal, bool isExportPassJournal, DateTime minDate, DateTime maxDate, string path)
		{
			SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					rubezhService.ExportJournalA(clientUID, isExportJournal, isExportPassJournal, minDate, maxDate, path);
			}, "ExportJournal");
		}

		public void ExportOrganisation(Guid clientUID, bool isWithDeleted, Guid organisationUid, string path)
		{
			SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					rubezhService.ExportOrganisationA(clientUID, isWithDeleted, organisationUid, path);
			}, "ExportOrganisation");
		}

		public void ExportOrganisationList(Guid clientUID, bool isWithDeleted, string path)
		{
			SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					rubezhService.ExportOrganisationListA(clientUID, isWithDeleted, path);
			}, "ExportOrganisationList");
		}

		public void ExportConfiguration(Guid clientUID, bool isExportDevices, bool isExportDoors, bool isExportZones, string path)
		{
			SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					rubezhService.ExportConfigurationA(clientUID, isExportDevices, isExportDoors, isExportZones, path);
			}, "ExportConfiguration");
		}

		public void ImportOrganisation(Guid clientUID, bool isWithDeleted, string path)
		{
			SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					rubezhService.ImportOrganisationA(clientUID, isWithDeleted, path);
			}, "ImportOrganisation");
		}

		public void ImportOrganisationList(Guid clientUID, bool isWithDeleted, string path)
		{
			SafeOperationCall(() =>
			{
				var rubezhService = RubezhServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (rubezhService as IDisposable)
					rubezhService.ImportOrganisationListA(clientUID, isWithDeleted, path);
			}, "ImportOrganisationList");
		}

		/////////////////////////////////////////

		public OperationResult<bool> RunProcedure(Guid procedureUID, List<Argument> args)
		{
			return RunProcedure(RubezhServiceFactory.UID, procedureUID, args);
		}
		public void ProcedureCallbackResponse(Guid procedureThreadUID, object value)
		{
			ProcedureCallbackResponse(RubezhServiceFactory.UID, procedureThreadUID, value);
		}
		public ProcedureProperties GetProperties(Guid layoutUID)
		{
			return GetProperties(RubezhServiceFactory.UID, layoutUID);
		}

		public void SetVariableValue(Guid variableUid, object value)
		{
			SetVariableValue(RubezhServiceFactory.UID, variableUid, value);
		}

		public void AddJournalItem(string message)
		{
			AddJournalItem(RubezhServiceFactory.UID, message);
		}

		public void ControlGKDevice(Guid deviceUid, GKStateBit command)
		{
			ControlGKDevice(RubezhServiceFactory.UID, deviceUid, command);
		}

		public void StartRecord(Guid cameraUid, Guid? journalItemUid, Guid? eventUid, int timeout)
		{
			StartRecord(RubezhServiceFactory.UID, cameraUid, journalItemUid, eventUid, timeout);
		}

		public void StopRecord(Guid cameraUid, Guid eventUid)
		{
			StopRecord(RubezhServiceFactory.UID, cameraUid, eventUid);
		}

		public void Ptz(Guid cameraUid, int ptzNumber)
		{
			Ptz(RubezhServiceFactory.UID, cameraUid, ptzNumber);
		}

		public void RviAlarm(string name)
		{
			RviAlarm(RubezhServiceFactory.UID, name);
		}

		public void RviOpenWindow(string name, int x, int y, int monitorNumber, string login, string ip)
		{
			RviOpenWindow(RubezhServiceFactory.UID, name, x, y, monitorNumber, login, ip);
		}

		public void ControlFireZone(Guid uid, ZoneCommandType commandType)
		{
			ControlFireZone(RubezhServiceFactory.UID, uid, commandType);
		}

		public void ControlGuardZone(Guid uid, GuardZoneCommandType commandType)
		{
			ControlGuardZone(RubezhServiceFactory.UID, uid, commandType);
		}

		public void ControlDirection(Guid uid, DirectionCommandType commandType)
		{
			ControlDirection(RubezhServiceFactory.UID, uid, commandType);
		}

		public void ControlGKDoor(Guid uid, GKDoorCommandType commandType)
		{
			ControlGKDoor(RubezhServiceFactory.UID, uid, commandType);
		}

		public void ControlDelay(Guid uid, DelayCommandType commandType)
		{
			ControlDelay(RubezhServiceFactory.UID, uid, commandType);
		}

		public void ControlPumpStation(Guid uid, PumpStationCommandType commandType)
		{
			ControlPumpStation(RubezhServiceFactory.UID, uid, commandType);
		}

		public void ControlMPT(Guid uid, MPTCommandType commandType)
		{
			ControlMPT(RubezhServiceFactory.UID, uid, commandType);
		}

		public void ExportJournal(bool isExportJournal, bool isExportPassJournal, DateTime minDate, DateTime maxDate, string path)
		{
			ExportJournal(RubezhServiceFactory.UID, isExportJournal, isExportPassJournal, minDate, maxDate, path);
		}

		public void ExportOrganisation(bool isWithDeleted, Guid organisationUid, string path)
		{
			ExportOrganisation(RubezhServiceFactory.UID, isWithDeleted, organisationUid, path);
		}

		public void ExportOrganisationList(bool isWithDeleted, string path)
		{
			ExportOrganisationList(RubezhServiceFactory.UID, isWithDeleted, path);
		}

		public void ExportConfiguration(bool isExportDevices, bool isExportDoors, bool isExportZones, string path)
		{
			ExportConfiguration(RubezhServiceFactory.UID, isExportDevices, isExportDoors, isExportZones, path);
		}

		public void ImportOrganisation(bool isWithDeleted, string path)
		{
			ImportOrganisation(RubezhServiceFactory.UID, isWithDeleted, path);
		}

		public void ImportOrganisationList(bool isWithDeleted, string path)
		{
			ImportOrganisationList(RubezhServiceFactory.UID, isWithDeleted, path);
		}

	}
}