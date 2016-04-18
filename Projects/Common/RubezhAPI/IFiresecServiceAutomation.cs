using RubezhAPI.Automation;
using RubezhAPI.AutomationCallback;
using RubezhAPI.GK;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using Common;

namespace RubezhAPI
{
	[ServiceContract]
	public interface IFiresecServiceAutomation
	{
		[OperationContract]
		OperationResult<bool> RunProcedure(Guid clientUID, Guid procedureUID, List<Argument> args);
		[OperationContract]
		[ServiceKnownType(typeof(Color))]
		void ProcedureCallbackResponse(Guid clientUID, Guid callbackUID, object value);
		[OperationContract]
		ProcedureProperties GetProperties(Guid clientUID, Guid layoutUID);
		[OperationContract]
		[ServiceKnownType(typeof(Color))]
		void SetVariableValue(Guid clientUID, Guid variableUid, object value);
		[OperationContract]
		void AddJournalItemA(Guid clientUID, string message, Guid? objectUID = null);
		[OperationContract]
		void ControlGKDevice(Guid clientUID, Guid deviceUid, GKStateBit command);
		[OperationContract]
		void StartRecord(Guid clientUID, Guid cameraUid, Guid? journalItemUid, Guid? eventUid, int timeout);
		[OperationContract]
		void StopRecord(Guid clientUID, Guid cameraUid, Guid eventUid);
		[OperationContract]
		void Ptz(Guid clientUID, Guid cameraUid, int ptzNumber);
		[OperationContract]
		void RviAlarm(Guid clientUID, string name);
		[OperationContract]
		void RviOpenWindow(Guid clientUid, string name, int x, int y, int monitorNumber, string login, string ip);
		[OperationContract]
		void ControlFireZone(Guid clientUID, Guid uid, ZoneCommandType commandType);
		[OperationContract]
		void ControlGuardZone(Guid clientUID, Guid uid, GuardZoneCommandType commandType);
		[OperationContract]
		void ControlDirection(Guid clientUID, Guid uid, DirectionCommandType commandType);
		[OperationContract]
		void ControlGKDoor(Guid clientUID, Guid uid, GKDoorCommandType commandType);
		[OperationContract]
		void ControlDelay(Guid clientUID, Guid uid, DelayCommandType commandType);
		[OperationContract]
		void ControlPumpStation(Guid clientUID, Guid uid, PumpStationCommandType commandType);
		[OperationContract]
		void ControlMPT(Guid clientUID, Guid uid, MPTCommandType commandType);
		[OperationContract]
		void ExportJournalA(Guid clientUID, bool isExportJournal, bool isExportPassJournal, DateTime minDate, DateTime maxDate, string path);
		[OperationContract]
		void ExportOrganisationA(Guid clientUID, bool isWithDeleted, Guid organisationUid, string path);
		[OperationContract]
		void ExportOrganisationListA(Guid clientUID, bool isWithDeleted, string path);
		[OperationContract]
		void ExportConfigurationA(Guid clientUID, bool isExportDevices, bool isExportDoors, bool isExportZones, string path);
		[OperationContract]
		void ImportOrganisationA(Guid clientUID, bool isWithDeleted, string path);
		[OperationContract]
		void ImportOrganisationListA(Guid clientUID, bool isWithDeleted, string path);
	}
}