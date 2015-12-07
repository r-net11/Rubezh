using Common;
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
			return SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.RunProcedure(FiresecServiceFactory.UID, procedureUID, args);
			});
		}
		public void ProcedureCallbackResponse(Guid procedureThreadUID, object value)
		{
			SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ProcedureCallbackResponse(procedureThreadUID, value);
			});
		}
		public ProcedureProperties GetProperties(Guid layoutUID)
		{
			return SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetProperties(layoutUID);
			});
		}

		public Variable GetVariable(Guid variableUid)
		{
			return SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					return firesecService.GetVariable(variableUid);
			});
		}

		public void SetVariableValue(Guid variableUid, object value)
		{
			SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.SetVariableValue(variableUid, value);
			});
		}

		public void AddJournalItem(string message)
		{
			SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.AddJournalItemA(message);
			});
		}

		public void ControlGKDevice(Guid deviceUid, GKStateBit command)
		{
			SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ControlGKDevice(deviceUid, command);
			});
		}

		public void StartRecord(Guid cameraUid, Guid? journalItemUid, Guid? eventUid, int timeout)
		{
			SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.StartRecord(cameraUid, journalItemUid, eventUid, timeout);
			});
		}

		public void StopRecord(Guid cameraUid, Guid eventUid)
		{
			SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.StopRecord(cameraUid, eventUid);
			});
		}

		public void Ptz(Guid cameraUid, int ptzNumber)
		{
			SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.Ptz(cameraUid, ptzNumber);
			});
		}

		public void RviAlarm(string name)
		{
			SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.RviAlarm(name);
			});
		}

		public void ControlFireZone(Guid uid, ZoneCommandType commandType)
		{
			SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ControlFireZone(uid, commandType);
			});
		}

		public void ControlGuardZone(Guid uid, GuardZoneCommandType commandType)
		{
			SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ControlGuardZone(uid, commandType);
			});
		}

		public void ControlDirection(Guid uid, DirectionCommandType commandType)
		{
			SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ControlDirection(uid, commandType);
			});
		}

		public void ControlGKDoor(Guid uid, GKDoorCommandType commandType)
		{
			SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ControlGKDoor(uid, commandType);
			});
		}

		public void ControlDelay(Guid uid, DelayCommandType commandType)
		{
			SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ControlDelay(uid, commandType);
			});
		}

		public void ControlPumpStation(Guid uid, PumpStationCommandType commandType)
		{
			SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ControlPumpStation(uid, commandType);
			});
		}

		public void ControlMPT(Guid uid, MPTCommandType commandType)
		{
			SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ControlMPT(uid, commandType);
			});
		}

		public void ExportJournal(bool isExportJournal, bool isExportPassJournal, DateTime minDate, DateTime maxDate, string path)
		{
			SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ExportJournalA(isExportJournal, isExportPassJournal, minDate, maxDate, path);
			});
		}

		public void ExportOrganisation(bool isWithDeleted, Guid organisationUid, string path)
		{
			SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ExportOrganisationA(isWithDeleted, organisationUid, path);
			});
		}

		public void ExportOrganisationList(bool isWithDeleted, string path)
		{
			SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ExportOrganisationListA(isWithDeleted, path);
			});
		}

		public void ExportConfiguration(bool isExportDevices, bool isExportDoors, bool isExportZones, string path)
		{
			SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ExportConfigurationA(isExportDevices, isExportDoors, isExportZones, path);
			});
		}

		public void ImportOrganisation(bool isWithDeleted, string path)
		{
			SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ImportOrganisationA(isWithDeleted, path);
			});
		}

		public void ImportOrganisationList(bool isWithDeleted, string path)
		{
			SafeContext.Execute(() =>
			{
				var firesecService = FiresecServiceFactory.Create(TimeSpan.FromMinutes(10));
				using (firesecService as IDisposable)
					firesecService.ImportOrganisationListA(isWithDeleted, path);
			});
		}

	}
}