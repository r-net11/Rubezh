using RubezhAPI;
using RubezhAPI.Automation;
using RubezhAPI.AutomationCallback;
using RubezhAPI.GK;
using System;
using System.Collections.Generic;

namespace RubezhClient
{
	public partial class SafeFiresecService
	{
		public OperationResult<bool> RunProcedure(Guid clientUID, Guid procedureUID, List<Argument> args)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.RunProcedure(clientUID, procedureUID, args);
			}, "RunProcedure");
		}
		public void ProcedureCallbackResponse(Guid clientUID, Guid procedureThreadUID, object value)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ProcedureCallbackResponse(clientUID, procedureThreadUID, value);
			}, "ProcedureCallbackResponse");
		}
		public ProcedureProperties GetProperties(Guid clientUID, Guid layoutUID)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetProperties(clientUID, layoutUID);
			}, "GetProperties");
		}

		public void SetVariableValue(Guid clientUID, Guid variableUid, object value)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.SetVariableValue(clientUID, variableUid, value);
			}, "SetVariableValue");
		}

		public void AddJournalItem(Guid clientUID, string message, Guid? objectUID = null)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.AddJournalItemA(clientUID, message, objectUID);
			}, "AddJournalItem");
		}

		public void ControlGKDevice(Guid clientUID, Guid deviceUid, GKStateBit command)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ControlGKDevice(clientUID, deviceUid, command);
			}, "ControlGKDevice");
		}

		public void StartRecord(Guid clientUID, Guid cameraUid, Guid? journalItemUid, Guid? eventUid, int timeout)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.StartRecord(clientUID, cameraUid, journalItemUid, eventUid, timeout);
			}, "StartRecord");
		}

		public void StopRecord(Guid clientUID, Guid cameraUid, Guid eventUid)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.StopRecord(clientUID, cameraUid, eventUid);
			}, "StopRecord");
		}

		public void Ptz(Guid clientUID, Guid cameraUid, int ptzNumber)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.Ptz(clientUID, cameraUid, ptzNumber);
			}, "Ptz");
		}

		public void RviAlarm(Guid clientUID, string name)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.RviAlarm(clientUID, name);
			}, "RviAlarm");
		}

		public void RviOpenWindow(Guid clientUid, string name, int x, int y, int monitorNumber, string login, string ip)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.RviOpenWindow(clientUid, name, x, y, monitorNumber, login, ip);
			}, "RviOpenWindow");
		}

		public void ControlFireZone(Guid clientUID, Guid uid, ZoneCommandType commandType)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ControlFireZone(clientUID, uid, commandType);
			}, "ControlFireZone");
		}

		public void ControlGuardZone(Guid clientUID, Guid uid, GuardZoneCommandType commandType)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ControlGuardZone(clientUID, uid, commandType);
			}, "ControlGuardZone");
		}

		public void ControlDirection(Guid clientUID, Guid uid, DirectionCommandType commandType)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ControlDirection(clientUID, uid, commandType);
			}, "ControlDirection");
		}

		public void ControlGKDoor(Guid clientUID, Guid uid, GKDoorCommandType commandType)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ControlGKDoor(clientUID, uid, commandType);
			}, "ControlGKDoor");
		}

		public void ControlDelay(Guid clientUID, Guid uid, DelayCommandType commandType)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ControlDelay(clientUID, uid, commandType);
			}, "ControlDelay");
		}

		public void ControlPumpStation(Guid clientUID, Guid uid, PumpStationCommandType commandType)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ControlPumpStation(clientUID, uid, commandType);
			}, "ControlPumpStation");
		}

		public void ControlMPT(Guid clientUID, Guid uid, MPTCommandType commandType)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ControlMPT(clientUID, uid, commandType);
			}, "ControlMPT");
		}

		public void ExportJournal(Guid clientUID, bool isExportJournal, bool isExportPassJournal, DateTime minDate, DateTime maxDate, string path)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ExportJournalA(clientUID, isExportJournal, isExportPassJournal, minDate, maxDate, path);
			}, "ExportJournal");
		}

		public void ExportOrganisation(Guid clientUID, bool isWithDeleted, Guid organisationUid, string path)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ExportOrganisationA(clientUID, isWithDeleted, organisationUid, path);
			}, "ExportOrganisation");
		}

		public void ExportOrganisationList(Guid clientUID, bool isWithDeleted, string path)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ExportOrganisationListA(clientUID, isWithDeleted, path);
			}, "ExportOrganisationList");
		}

		public void ExportConfiguration(Guid clientUID, bool isExportDevices, bool isExportDoors, bool isExportZones, string path)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ExportConfigurationA(clientUID, isExportDevices, isExportDoors, isExportZones, path);
			}, "ExportConfiguration");
		}

		public void ImportOrganisation(Guid clientUID, bool isWithDeleted, string path)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ImportOrganisationA(clientUID, isWithDeleted, path);
			}, "ImportOrganisation");
		}

		public void ImportOrganisationList(Guid clientUID, bool isWithDeleted, string path)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ImportOrganisationListA(clientUID, isWithDeleted, path);
			}, "ImportOrganisationList");
		}

		/////////////////////////////////////////

		public OperationResult<bool> RunProcedure(Guid procedureUID, List<Argument> args)
		{
			return RunProcedure(FiresecServiceFactory.UID, procedureUID, args);
		}
		public void ProcedureCallbackResponse(Guid procedureThreadUID, object value)
		{
			ProcedureCallbackResponse(FiresecServiceFactory.UID, procedureThreadUID, value);
		}
		public ProcedureProperties GetProperties(Guid layoutUID)
		{
			return GetProperties(FiresecServiceFactory.UID, layoutUID);
		}

		public void SetVariableValue(Guid variableUid, object value)
		{
			SetVariableValue(FiresecServiceFactory.UID, variableUid, value);
		}

		public void AddJournalItem(string message)
		{
			AddJournalItem(FiresecServiceFactory.UID, message);
		}

		public void ControlGKDevice(Guid deviceUid, GKStateBit command)
		{
			ControlGKDevice(FiresecServiceFactory.UID, deviceUid, command);
		}

		public void StartRecord(Guid cameraUid, Guid? journalItemUid, Guid? eventUid, int timeout)
		{
			StartRecord(FiresecServiceFactory.UID, cameraUid, journalItemUid, eventUid, timeout);
		}

		public void StopRecord(Guid cameraUid, Guid eventUid)
		{
			StopRecord(FiresecServiceFactory.UID, cameraUid, eventUid);
		}

		public void Ptz(Guid cameraUid, int ptzNumber)
		{
			Ptz(FiresecServiceFactory.UID, cameraUid, ptzNumber);
		}

		public void RviAlarm(string name)
		{
			RviAlarm(FiresecServiceFactory.UID, name);
		}

		public void RviOpenWindow(string name, int x, int y, int monitorNumber, string login, string ip)
		{
			RviOpenWindow(FiresecServiceFactory.UID, name, x, y, monitorNumber, login, ip);
		}

		public void ControlFireZone(Guid uid, ZoneCommandType commandType)
		{
			ControlFireZone(FiresecServiceFactory.UID, uid, commandType);
		}

		public void ControlGuardZone(Guid uid, GuardZoneCommandType commandType)
		{
			ControlGuardZone(FiresecServiceFactory.UID, uid, commandType);
		}

		public void ControlDirection(Guid uid, DirectionCommandType commandType)
		{
			ControlDirection(FiresecServiceFactory.UID, uid, commandType);
		}

		public void ControlGKDoor(Guid uid, GKDoorCommandType commandType)
		{
			ControlGKDoor(FiresecServiceFactory.UID, uid, commandType);
		}

		public void ControlDelay(Guid uid, DelayCommandType commandType)
		{
			ControlDelay(FiresecServiceFactory.UID, uid, commandType);
		}

		public void ControlPumpStation(Guid uid, PumpStationCommandType commandType)
		{
			ControlPumpStation(FiresecServiceFactory.UID, uid, commandType);
		}

		public void ControlMPT(Guid uid, MPTCommandType commandType)
		{
			ControlMPT(FiresecServiceFactory.UID, uid, commandType);
		}

		public void ExportJournal(bool isExportJournal, bool isExportPassJournal, DateTime minDate, DateTime maxDate, string path)
		{
			ExportJournal(FiresecServiceFactory.UID, isExportJournal, isExportPassJournal, minDate, maxDate, path);
		}

		public void ExportOrganisation(bool isWithDeleted, Guid organisationUid, string path)
		{
			ExportOrganisation(FiresecServiceFactory.UID, isWithDeleted, organisationUid, path);
		}

		public void ExportOrganisationList(bool isWithDeleted, string path)
		{
			ExportOrganisationList(FiresecServiceFactory.UID, isWithDeleted, path);
		}

		public void ExportConfiguration(bool isExportDevices, bool isExportDoors, bool isExportZones, string path)
		{
			ExportConfiguration(FiresecServiceFactory.UID, isExportDevices, isExportDoors, isExportZones, path);
		}

		public void ImportOrganisation(bool isWithDeleted, string path)
		{
			ImportOrganisation(FiresecServiceFactory.UID, isWithDeleted, path);
		}

		public void ImportOrganisationList(bool isWithDeleted, string path)
		{
			ImportOrganisationList(FiresecServiceFactory.UID, isWithDeleted, path);
		}

	}
}