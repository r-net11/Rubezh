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
		public OperationResult<bool> RunProcedure(Guid procedureUID, List<Argument> args)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.RunProcedure(FiresecServiceFactory.UID, procedureUID, args);
			}, "RunProcedure");
		}
		public void ProcedureCallbackResponse(Guid procedureThreadUID, object value)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ProcedureCallbackResponse(procedureThreadUID, value);
			}, "ProcedureCallbackResponse");
		}
		public ProcedureProperties GetProperties(Guid layoutUID)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetProperties(layoutUID);
			}, "GetProperties");
		}

		public Variable GetVariable(Guid variableUid)
		{
			return SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetVariable(variableUid);
			}, "GetVariable");
		}

		public void SetVariableValue(Guid variableUid, object value)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.SetVariableValue(variableUid, value);
			}, "SetVariableValue");
		}

		public void AddJournalItem(string message)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.AddJournalItemA(message);
			}, "AddJournalItem");
		}

		public void ControlGKDevice(Guid deviceUid, GKStateBit command)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ControlGKDevice(deviceUid, command);
			}, "ControlGKDevice");
		}

		public void StartRecord(Guid cameraUid, Guid? journalItemUid, Guid? eventUid, int timeout)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.StartRecord(cameraUid, journalItemUid, eventUid, timeout);
			}, "StartRecord");
		}

		public void StopRecord(Guid cameraUid, Guid eventUid)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.StopRecord(cameraUid, eventUid);
			}, "StopRecord");
		}

		public void Ptz(Guid cameraUid, int ptzNumber)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.Ptz(cameraUid, ptzNumber);
			}, "Ptz");
		}

		public void RviAlarm(string name)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.RviAlarm(name);
			}, "RviAlarm");
		}

		public void ControlFireZone(Guid uid, ZoneCommandType commandType)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ControlFireZone(uid, commandType);
			}, "ControlFireZone");
		}

		public void ControlGuardZone(Guid uid, GuardZoneCommandType commandType)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ControlGuardZone(uid, commandType);
			}, "ControlGuardZone");
		}

		public void ControlDirection(Guid uid, DirectionCommandType commandType)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ControlDirection(uid, commandType);
			}, "ControlDirection");
		}

		public void ControlGKDoor(Guid uid, GKDoorCommandType commandType)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ControlGKDoor(uid, commandType);
			}, "ControlGKDoor");
		}

		public void ControlDelay(Guid uid, DelayCommandType commandType)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ControlDelay(uid, commandType);
			}, "ControlDelay");
		}

		public void ControlPumpStation(Guid uid, PumpStationCommandType commandType)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ControlPumpStation(uid, commandType);
			}, "ControlPumpStation");
		}

		public void ControlMPT(Guid uid, MPTCommandType commandType)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ControlMPT(uid, commandType);
			}, "ControlMPT");
		}

		public void ExportJournal(bool isExportJournal, bool isExportPassJournal, DateTime minDate, DateTime maxDate, string path)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ExportJournalA(isExportJournal, isExportPassJournal, minDate, maxDate, path);
			}, "ExportJournal");
		}

		public void ExportOrganisation(bool isWithDeleted, Guid organisationUid, string path)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ExportOrganisationA(isWithDeleted, organisationUid, path);
			}, "ExportOrganisation");
		}

		public void ExportOrganisationList(bool isWithDeleted, string path)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ExportOrganisationListA(isWithDeleted, path);
			}, "ExportOrganisationList");
		}

		public void ExportConfiguration(bool isExportDevices, bool isExportDoors, bool isExportZones, string path)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ExportConfigurationA(isExportDevices, isExportDoors, isExportZones, path);
			}, "ExportConfiguration");
		}

		public void ImportOrganisation(bool isWithDeleted, string path)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ImportOrganisationA(isWithDeleted, path);
			}, "ImportOrganisation");
		}

		public void ImportOrganisationList(bool isWithDeleted, string path)
		{
			SafeOperationCall(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ImportOrganisationListA(isWithDeleted, path);
			}, "ImportOrganisationList");
		}

	}
}