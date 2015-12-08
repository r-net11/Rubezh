using RubezhAPI.Automation;
using RubezhAPI.AutomationCallback;
using RubezhAPI.GK;
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Windows.Media;

namespace RubezhAPI
{
	[ServiceContract]
	public interface IFiresecServiceAutomation
	{
		[OperationContract]
		OperationResult<bool> RunProcedure(Guid clientUID, Guid procedureUID, List<Argument> args);
		[OperationContract(IsOneWay = true)]
		[ServiceKnownType(typeof(Color))]
		void ProcedureCallbackResponse(Guid callbackUID, object value, Guid clientUID);
		[OperationContract]
		ProcedureProperties GetProperties(Guid layoutUID, Guid clientUID);
		[OperationContract(IsOneWay = true)]
		[ServiceKnownType(typeof(Color))]
		void SetVariableValue(Guid variableUid, object value, Guid clientUID);
		[OperationContract]
		Variable GetVariable(Guid variableUid, Guid clientUID);
		[OperationContract]
		void AddJournalItemA(string message, Guid clientUID);
		[OperationContract]
		void ControlGKDevice(Guid deviceUid, GKStateBit command, Guid clientUID);
		[OperationContract]
		void StartRecord(Guid cameraUid, Guid? journalItemUid, Guid? eventUid, int timeout, Guid clientUID);
		[OperationContract]
		void StopRecord(Guid cameraUid, Guid eventUid, Guid clientUID);
		[OperationContract]
		void Ptz(Guid cameraUid, int ptzNumber, Guid clientUID);
		[OperationContract]
		void RviAlarm(string name, Guid clientUID);
		[OperationContract]
		void ControlFireZone(Guid uid, ZoneCommandType commandType, Guid clientUID);
		[OperationContract]
		void ControlGuardZone(Guid uid, GuardZoneCommandType commandType, Guid clientUID);
		[OperationContract]
		void ControlDirection(Guid uid, DirectionCommandType commandType, Guid clientUID);
		[OperationContract]
		void ControlGKDoor(Guid uid, GKDoorCommandType commandType, Guid clientUID);
		[OperationContract]
		void ControlDelay(Guid uid, DelayCommandType commandType, Guid clientUID);
		[OperationContract]
		void ControlPumpStation(Guid uid, PumpStationCommandType commandType, Guid clientUID);
		[OperationContract]
		void ControlMPT(Guid uid, MPTCommandType commandType, Guid clientUID);
		[OperationContract]
		void ExportJournalA(bool isExportJournal, bool isExportPassJournal, DateTime minDate, DateTime maxDate, string path, Guid clientUID);
		[OperationContract]
		void ExportOrganisationA(bool isWithDeleted, Guid organisationUid, string path, Guid clientUID);
		[OperationContract]
		void ExportOrganisationListA(bool isWithDeleted, string path, Guid clientUID);
		[OperationContract]
		void ExportConfigurationA(bool isExportDevices, bool isExportDoors, bool isExportZones, string path, Guid clientUID);
		[OperationContract]
		void ImportOrganisationA(bool isWithDeleted, string path, Guid clientUID);
		[OperationContract]
		void ImportOrganisationListA(bool isWithDeleted, string path, Guid clientUID);
	}
}