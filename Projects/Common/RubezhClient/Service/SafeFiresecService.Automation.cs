using System;
using System.Collections.Generic;
using Common;
using RubezhAPI;
using RubezhAPI.Automation;
using RubezhAPI.AutomationCallback;
using RubezhAPI.GK;
using OpcClientSdk;
using OpcClientSdk.Da;

namespace RubezhClient
{
	public partial class SafeFiresecService
	{
		public OperationResult<bool> RunProcedure(Guid procedureUID, List<Argument> args)
		{
			return SafeContext.Execute(() => FiresecService.RunProcedure(FiresecServiceFactory.UID, procedureUID, args));
		}
		public void ProcedureCallbackResponse(Guid procedureThreadUID, object value)
		{
			SafeContext.Execute(() => FiresecService.ProcedureCallbackResponse(procedureThreadUID, value));
		}
		public ProcedureProperties GetProperties(Guid layoutUID)
		{
			return SafeContext.Execute(() => FiresecService.GetProperties(layoutUID));
		}

		public Variable GetVariable(Guid variableUid)
		{
			return SafeContext.Execute(() => FiresecService.GetVariable(variableUid));
		}

		public void SetVariableValue(Guid variableUid, object value)
		{
			SafeContext.Execute(() => FiresecService.SetVariableValue(variableUid, value));
		}

		public void AddJournalItem(string message)
		{
			SafeContext.Execute(() => FiresecService.AddJournalItemA(message));
		}

		public void ControlGKDevice(Guid deviceUid, GKStateBit command)
		{
			SafeContext.Execute(() => FiresecService.ControlGKDevice(deviceUid, command));
		}

		public void StartRecord(Guid cameraUid, Guid? journalItemUid, Guid? eventUid, int timeout)
		{
			SafeContext.Execute(() => FiresecService.StartRecord(cameraUid, journalItemUid, eventUid, timeout));
		}

		public void StopRecord(Guid cameraUid, Guid eventUid)
		{
			SafeContext.Execute(() => FiresecService.StopRecord(cameraUid, eventUid));
		}

		public void Ptz(Guid cameraUid, int ptzNumber)
		{
			SafeContext.Execute(() => FiresecService.Ptz(cameraUid, ptzNumber));
		}

		public void RviAlarm(string name)
		{
			SafeContext.Execute(() => FiresecService.RviAlarm(name));
		}

		public void ControlFireZone(Guid uid, ZoneCommandType commandType)
		{
			SafeContext.Execute(() => FiresecService.ControlFireZone(uid, commandType));
		}

		public void ControlGuardZone(Guid uid, GuardZoneCommandType commandType)
		{
			SafeContext.Execute(() => FiresecService.ControlGuardZone(uid, commandType));
		}

		public void ControlDirection(Guid uid, DirectionCommandType commandType)
		{
			SafeContext.Execute(() => FiresecService.ControlDirection(uid, commandType));
		}

		public void ControlGKDoor(Guid uid, GKDoorCommandType commandType)
		{
			SafeContext.Execute(() => FiresecService.ControlGKDoor(uid, commandType));
		}

		public void ControlDelay(Guid uid, DelayCommandType commandType)
		{
			SafeContext.Execute(() => FiresecService.ControlDelay(uid, commandType));
		}

		public void ControlPumpStation(Guid uid, PumpStationCommandType commandType)
		{
			SafeContext.Execute(() => FiresecService.ControlPumpStation(uid, commandType));
		}

		public void ControlMPT(Guid uid, MPTCommandType commandType)
		{
			SafeContext.Execute(() => FiresecService.ControlMPT(uid, commandType));
		}

		public void ExportJournal(bool isExportJournal, bool isExportPassJournal, DateTime minDate, DateTime maxDate, string path)
		{
			SafeContext.Execute(() => FiresecService.ExportJournalA(isExportJournal, isExportPassJournal, minDate, maxDate, path));
		}

		public void ExportOrganisation(bool isWithDeleted, Guid organisationUid, string path)
		{
			SafeContext.Execute(() => FiresecService.ExportOrganisationA(isWithDeleted, organisationUid, path));
		}

		public void ExportOrganisationList(bool isWithDeleted, string path)
		{
			SafeContext.Execute(() => FiresecService.ExportOrganisationListA(isWithDeleted, path));
		}

		public void ExportConfiguration(bool isExportDevices, bool isExportDoors, bool isExportZones, string path)
		{
			SafeContext.Execute(() => FiresecService.ExportConfigurationA(isExportDevices, isExportDoors, isExportZones, path));
		}

		public void ImportOrganisation(bool isWithDeleted, string path)
		{
			SafeContext.Execute(() => FiresecService.ImportOrganisationA(isWithDeleted, path));
		}
		
		public void ImportOrganisationList(bool isWithDeleted, string path)
		{
			SafeContext.Execute(() => FiresecService.ImportOrganisationListA(isWithDeleted, path));
		}

		#region OPC DA Server

		public OperationResult<OpcDaServer[]> GetOpcDaServers()
		{
			return SafeContext.Execute(() => FiresecService.GetOpcDaServers());
		}

		public OperationResult ConnectToOpcDaServer(OpcDaServer server)
		{
			return SafeContext.Execute(() => FiresecService.ConnectToOpcDaServer(server));
		}

		public OperationResult DisconnectFromOpcDaServer(OpcDaServer server)
		{
			return SafeContext.Execute(() => FiresecService.DisconnectFromOpcDaServer(server));
		}

		public OperationResult<OpcDaElement[]> GetOpcDaServerGroupAndTags(OpcDaServer server)
		{
			return SafeContext.Execute(() => FiresecService.GetOpcDaServerGroupAndTags(server));
		}

		public OperationResult<OpcServerStatus> GetOpcDaServerStatus(OpcDaServer server)
		{
			return SafeContext.Execute(() => FiresecService.GetOpcDaServerStatus(server));
		}

		public OperationResult<TsCDaItemValueResult[]> ReadOpcDaServerTags(OpcDaServer server)
		{
			return SafeContext.Execute(() => FiresecService.ReadOpcDaServerTags(server));
		}
		
		public OperationResult WriteOpcDaServerTags(OpcDaServer server, TsCDaItemValue[] tagValues)
		{ 
			return SafeContext.Execute(() => FiresecService.WriteOpcDaServerTags(server, tagValues));
		}

		#endregion
	}
}