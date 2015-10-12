using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Windows.Media;
using RubezhAPI.Automation;
using RubezhAPI.AutomationCallback;
using RubezhAPI.GK;

namespace RubezhAPI
{
	[ServiceContract]
	public interface IFiresecServiceAutomation
	{
		[OperationContract]
		OperationResult<bool> RunProcedure(Guid clientUID, Guid procedureUID, List<Argument> args);
		[OperationContract(IsOneWay = true)]
		[ServiceKnownType(typeof(Color))]
		void ProcedureCallbackResponse(Guid callbackUID, object value);
		[OperationContract]
		ProcedureProperties GetProperties(Guid layoutUID);
		[OperationContract(IsOneWay = true)]
		[ServiceKnownType(typeof(Color))]
		void SetVariableValue(Guid variableUid, object value);
		[OperationContract]
		Variable GetVariable(Guid variableUid);
		[OperationContract]
		void AddJournalItemA(string message);
		[OperationContract]
		void ControlGKDevice(Guid deviceUid, GKStateBit command);
		[OperationContract]
		void StartRecord(Guid cameraUid, Guid? journalItemUid, Guid? eventUid, int timeout);
		[OperationContract]
		void StopRecord(Guid cameraUid, Guid eventUid);
		[OperationContract]
		void Ptz(Guid cameraUid, int ptzNumber);
		[OperationContract]
		void RviAlarm(string name);
		[OperationContract]
		void ControlFireZone(Guid uid, ZoneCommandType commandType);
		[OperationContract]
		void ControlGuardZone(Guid uid, GuardZoneCommandType commandType);
		[OperationContract]
		void ControlDirection(Guid uid, DirectionCommandType commandType);
		[OperationContract]
		void ControlGKDoor(Guid uid, GKDoorCommandType commandType);
		[OperationContract]
		void ControlDelay(Guid uid, DelayCommandType commandType);
		[OperationContract]
		void ExportJournalA(bool isExportJournal, bool isExportPassJournal, DateTime minDate, DateTime maxDate, string path);
		[OperationContract]
		void ExportOrganisationA(bool isWithDeleted, Guid organisationUid, string path);
		[OperationContract]
		void ExportOrganisationListA(bool isWithDeleted, string path);
		[OperationContract]
		void ExportConfigurationA(bool isExportDevices, bool isExportDoors, bool isExportZones, string path);
		[OperationContract]
		void ImportOrganisationA(bool isWithDeleted, string path);
		[OperationContract]
		void ImportOrganisationListA(bool isWithDeleted, string path);
	}
}