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
		public OperationResult<bool> RunProcedure(Guid procedureUID, List<Argument> args, Guid clientUID)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.RunProcedure(clientUID, procedureUID, args);
			}, "RunProcedure");
		}
		public void ProcedureCallbackResponse(Guid procedureThreadUID, object value, Guid clientUID)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ProcedureCallbackResponse(procedureThreadUID, value, clientUID);
			}, "ProcedureCallbackResponse");
		}
		public ProcedureProperties GetProperties(Guid layoutUID, Guid clientUID)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetProperties(layoutUID, clientUID);
			}, "GetProperties");
		}

		public Variable GetVariable(Guid variableUid, Guid clientUID)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetVariable(variableUid, clientUID);
			}, "GetVariable");
		}

		public void SetVariableValue(Guid variableUid, object value, Guid clientUID)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.SetVariableValue(variableUid, value, clientUID);
			}, "SetVariableValue");
		}

		public void AddJournalItem(string message, Guid clientUID)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.AddJournalItemA(message, clientUID);
			}, "AddJournalItem");
		}

		public void ControlGKDevice(Guid deviceUid, GKStateBit command, Guid clientUID)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ControlGKDevice(deviceUid, command, clientUID);
			}, "ControlGKDevice");
		}

		public void StartRecord(Guid cameraUid, Guid? journalItemUid, Guid? eventUid, int timeout, Guid clientUID)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.StartRecord(cameraUid, journalItemUid, eventUid, timeout, clientUID);
			}, "StartRecord");
		}

		public void StopRecord(Guid cameraUid, Guid eventUid, Guid clientUID)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.StopRecord(cameraUid, eventUid, clientUID);
			}, "StopRecord");
		}

		public void Ptz(Guid cameraUid, int ptzNumber, Guid clientUID)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.Ptz(cameraUid, ptzNumber, clientUID);
			}, "Ptz");
		}

		public void RviAlarm(string name, Guid clientUID)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.RviAlarm(name, clientUID);
			}, "RviAlarm");
		}

		public void ControlFireZone(Guid uid, ZoneCommandType commandType, Guid clientUID)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ControlFireZone(uid, commandType, clientUID);
			}, "ControlFireZone");
		}

		public void ControlGuardZone(Guid uid, GuardZoneCommandType commandType, Guid clientUID)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ControlGuardZone(uid, commandType, clientUID);
			}, "ControlGuardZone");
		}

		public void ControlDirection(Guid uid, DirectionCommandType commandType, Guid clientUID)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ControlDirection(uid, commandType, clientUID);
			}, "ControlDirection");
		}

		public void ControlGKDoor(Guid uid, GKDoorCommandType commandType, Guid clientUID)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ControlGKDoor(uid, commandType, clientUID);
			}, "ControlGKDoor");
		}

		public void ControlDelay(Guid uid, DelayCommandType commandType, Guid clientUID)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ControlDelay(uid, commandType, clientUID);
			}, "ControlDelay");
		}

		public void ControlPumpStation(Guid uid, PumpStationCommandType commandType, Guid clientUID)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ControlPumpStation(uid, commandType, clientUID);
			}, "ControlPumpStation");
		}

		public void ControlMPT(Guid uid, MPTCommandType commandType, Guid clientUID)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ControlMPT(uid, commandType, clientUID);
			}, "ControlMPT");
		}

		public void ExportJournal(bool isExportJournal, bool isExportPassJournal, DateTime minDate, DateTime maxDate, string path, Guid clientUID)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ExportJournalA(isExportJournal, isExportPassJournal, minDate, maxDate, path, clientUID);
			}, "ExportJournal");
		}

		public void ExportOrganisation(bool isWithDeleted, Guid organisationUid, string path, Guid clientUID)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ExportOrganisationA(isWithDeleted, organisationUid, path, clientUID);
			}, "ExportOrganisation");
		}

		public void ExportOrganisationList(bool isWithDeleted, string path, Guid clientUID)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ExportOrganisationListA(isWithDeleted, path, clientUID);
			}, "ExportOrganisationList");
		}

		public void ExportConfiguration(bool isExportDevices, bool isExportDoors, bool isExportZones, string path, Guid clientUID)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ExportConfigurationA(isExportDevices, isExportDoors, isExportZones, path, clientUID);
			}, "ExportConfiguration");
		}

		public void ImportOrganisation(bool isWithDeleted, string path, Guid clientUID)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ImportOrganisationA(isWithDeleted, path, clientUID);
			}, "ImportOrganisation");
		}

		public void ImportOrganisationList(bool isWithDeleted, string path, Guid clientUID)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ImportOrganisationListA(isWithDeleted, path, clientUID);
			}, "ImportOrganisationList");
		}

		/////////////////////////////////////////

		public OperationResult<bool> RunProcedure(Guid procedureUID, List<Argument> args)
		{
			return RunProcedure(procedureUID, args, FiresecServiceFactory.UID);
		}
		public void ProcedureCallbackResponse(Guid procedureThreadUID, object value)
		{
			ProcedureCallbackResponse(procedureThreadUID, value, FiresecServiceFactory.UID);
		}
		public ProcedureProperties GetProperties(Guid layoutUID)
		{
			return GetProperties(layoutUID, FiresecServiceFactory.UID);
		}

		public Variable GetVariable(Guid variableUid)
		{
			return GetVariable(variableUid, FiresecServiceFactory.UID);
		}

		public void SetVariableValue(Guid variableUid, object value)
		{
			SetVariableValue(variableUid, value, FiresecServiceFactory.UID);
		}

		public void AddJournalItem(string message)
		{
			AddJournalItem(message, FiresecServiceFactory.UID);
		}

		public void ControlGKDevice(Guid deviceUid, GKStateBit command)
		{
			ControlGKDevice(deviceUid, command, FiresecServiceFactory.UID);
		}

		public void StartRecord(Guid cameraUid, Guid? journalItemUid, Guid? eventUid, int timeout)
		{
			StartRecord(cameraUid, journalItemUid, eventUid, timeout, FiresecServiceFactory.UID);
		}

		public void StopRecord(Guid cameraUid, Guid eventUid)
		{
			StopRecord(cameraUid, eventUid, FiresecServiceFactory.UID);
		}

		public void Ptz(Guid cameraUid, int ptzNumber)
		{
			Ptz(cameraUid, ptzNumber, FiresecServiceFactory.UID);
		}

		public void RviAlarm(string name)
		{
			RviAlarm(name, FiresecServiceFactory.UID);
		}

		public void ControlFireZone(Guid uid, ZoneCommandType commandType)
		{
			ControlFireZone(uid, commandType, FiresecServiceFactory.UID);
		}

		public void ControlGuardZone(Guid uid, GuardZoneCommandType commandType)
		{
			ControlGuardZone(uid, commandType, FiresecServiceFactory.UID);
		}

		public void ControlDirection(Guid uid, DirectionCommandType commandType)
		{
			ControlDirection(uid, commandType, FiresecServiceFactory.UID);
		}

		public void ControlGKDoor(Guid uid, GKDoorCommandType commandType)
		{
			ControlGKDoor(uid, commandType, FiresecServiceFactory.UID);
		}

		public void ControlDelay(Guid uid, DelayCommandType commandType)
		{
			ControlDelay(uid, commandType, FiresecServiceFactory.UID);
		}

		public void ControlPumpStation(Guid uid, PumpStationCommandType commandType)
		{
			ControlPumpStation(uid, commandType, FiresecServiceFactory.UID);
		}

		public void ControlMPT(Guid uid, MPTCommandType commandType)
		{
			ControlMPT(uid, commandType, FiresecServiceFactory.UID);
		}

		public void ExportJournal(bool isExportJournal, bool isExportPassJournal, DateTime minDate, DateTime maxDate, string path)
		{
			ExportJournal(isExportJournal, isExportPassJournal, minDate, maxDate, path, FiresecServiceFactory.UID);
		}

		public void ExportOrganisation(bool isWithDeleted, Guid organisationUid, string path)
		{
			ExportOrganisation(isWithDeleted, organisationUid, path, FiresecServiceFactory.UID);
		}

		public void ExportOrganisationList(bool isWithDeleted, string path)
		{
			ExportOrganisationList(isWithDeleted, path, FiresecServiceFactory.UID);
		}

		public void ExportConfiguration(bool isExportDevices, bool isExportDoors, bool isExportZones, string path)
		{
			ExportConfiguration(isExportDevices, isExportDoors, isExportZones, path, FiresecServiceFactory.UID);
		}

		public void ImportOrganisation(bool isWithDeleted, string path)
		{
			ImportOrganisation(isWithDeleted, path, FiresecServiceFactory.UID);
		}

		public void ImportOrganisationList(bool isWithDeleted, string path)
		{
			ImportOrganisationList(isWithDeleted, path, FiresecServiceFactory.UID);
		}
	}
}