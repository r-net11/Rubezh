using Common;
using RubezhAPI.Automation;
using RubezhAPI.AutomationCallback;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhAPI.SKD;
using System;
using System.Collections.Generic;

namespace Infrastructure.Automation
{
	public partial class ProcedureExecutionContext
	{
		static GetSystemConfigurationDelegate _getSystemConfiguration;
		static Action<AutomationCallbackResult, Guid?> OnSendCallback;
		static Action<Guid, Guid, object> OnCallbackResponse;
		static Action<Guid, Variable> OnSynchronizeVariable;
		static Action<Guid, string, Guid?> OnAddJournalItem;
		static Action<Guid, Guid, GKStateBit> OnControlGKDevice;
		static Action<Guid, Guid, Guid?, Guid?, int> OnStartRecord;
		static Action<Guid, Guid, Guid> OnStopRecord;
		static Action<Guid, Guid, int> OnPtz;
		static Action<Guid, string> OnRviAlarm;
		static Action<Guid, string, int, int, int, string, string> OnRviOpenWindow;
		static Action<Guid, Guid, ZoneCommandType> OnControlFireZone;
		static Action<Guid, Guid, GuardZoneCommandType> OnControlGuardZone;
		static Action<Guid, Guid, DirectionCommandType> OnControlDirection;
		static Action<Guid, Guid, GKDoorCommandType> OnControlGKDoor;
		static Action<Guid, Guid, DelayCommandType> OnControlDelay;
		static Action<Guid, Guid, PumpStationCommandType> OnControlPumpStation;
		static Action<Guid, Guid, MPTCommandType> OnControlMPT;
		static Action<Guid, bool, bool, DateTime, DateTime, string> OnExportJournal;
		static Action<Guid, bool, Guid, string> OnExportOrganisation;
		static Action<Guid, bool, string> OnExportOrganisationList;
		static Action<Guid, bool, bool, bool, string> OnExportConfiguration;
		static Action<Guid, bool, string> OnImportOrganisation;
		static Action<Guid, bool, string> OnImportOrganisationList;
		static GetOrganisationsEventHandler OnGetOrganisations;

		public static void Initialize(ContextType contextType,
			GetSystemConfigurationDelegate getSystemConfiguration,
			Action<AutomationCallbackResult, Guid?> onSendCallback = null,
			Action<Guid, Guid, object> onCallbackResponse = null,
			Action<Guid, Variable> onSynchronizeVariable = null,
			Action<Guid, string, Guid?> onAddJournalItem = null,
			Action<Guid, Guid, GKStateBit> onControlGKDevice = null,
			Action<Guid, Guid, Guid?, Guid?, int> onStartRecord = null,
			Action<Guid, Guid, Guid> onStopRecord = null,
			Action<Guid, Guid, int> onPtz = null,
			Action<Guid, string> onRviAlarm = null,
			Action<Guid, string, int, int, int, string, string> onRviOpenWindow = null,
			Action<Guid, Guid, ZoneCommandType> onControlFireZone = null,
			Action<Guid, Guid, GuardZoneCommandType> onControlGuardZone = null,
			Action<Guid, Guid, DirectionCommandType> onControlDirection = null,
			Action<Guid, Guid, GKDoorCommandType> onControlGKDoor = null,
			Action<Guid, Guid, DelayCommandType> onControlDelay = null,
			Action<Guid, Guid, PumpStationCommandType> onControlPumpStation = null,
			Action<Guid, Guid, MPTCommandType> onControlMPT = null,
			Action<Guid, bool, bool, DateTime, DateTime, string> onExportJournal = null,
			Action<Guid, bool, Guid, string> onExportOrganisation = null,
			Action<Guid, bool, string> onExportOrganisationList = null,
			Action<Guid, bool, bool, bool, string> onExportConfiguration = null,
			Action<Guid, bool, string> onImportOrganisation = null,
			Action<Guid, bool, string> onImportOrganisationList = null,
			GetOrganisationsEventHandler onGetOrganisations = null
			)
		{
			ContextType = contextType;
			_getSystemConfiguration = getSystemConfiguration;
			OnSendCallback = onSendCallback;
			OnCallbackResponse = onCallbackResponse;
			OnSynchronizeVariable = onSynchronizeVariable;
			OnAddJournalItem = onAddJournalItem;
			OnControlGKDevice = onControlGKDevice;
			OnStartRecord = onStartRecord;
			OnStopRecord = onStopRecord;
			OnPtz = onPtz;
			OnRviAlarm = onRviAlarm;
			OnRviOpenWindow = onRviOpenWindow;
			OnControlFireZone = onControlFireZone;
			OnControlGuardZone = onControlGuardZone;
			OnControlDirection = onControlDirection;
			OnControlGKDoor = onControlGKDoor;
			OnControlDelay = onControlDelay;
			OnControlPumpStation = onControlPumpStation;
			OnControlMPT = onControlMPT;
			OnExportJournal = onExportJournal;
			OnExportOrganisation = onExportOrganisation;
			OnExportOrganisationList = onExportOrganisationList;
			OnExportConfiguration = onExportConfiguration;
			OnImportOrganisation = onImportOrganisation;
			OnImportOrganisationList = onImportOrganisationList;
			OnGetOrganisations = onGetOrganisations;

			GlobalVariables = SystemConfiguration != null ? Utils.Clone(SystemConfiguration.AutomationConfiguration.GlobalVariables) : new List<Variable>();
		}

		public static void SendCallback(AutomationCallbackResult callback, Guid? clientUID)
		{
			if (OnSendCallback != null)
				OnSendCallback(callback, clientUID);
		}

		public static void AddJournalItem(Guid clientUID, string message, Guid? objectUID = null)
		{
			if (OnAddJournalItem != null)
				OnAddJournalItem(clientUID, message, objectUID);
		}

		public static void ControlGKDevice(Guid clientUID, Guid deviceUid, GKStateBit command)
		{
			if (OnControlGKDevice != null)
				OnControlGKDevice(clientUID, deviceUid, command);
		}

		public static void StartRecord(Guid clientUID, Guid cameraUid, Guid? journalItemUid, Guid? eventUid, int timeout)
		{
			if (OnStartRecord != null)
				OnStartRecord(clientUID, cameraUid, journalItemUid, eventUid, timeout);
		}

		public static void StopRecord(Guid clientUID, Guid cameraUid, Guid eventUid)
		{
			if (OnStopRecord != null)
				OnStopRecord(clientUID, cameraUid, eventUid);
		}

		public static void Ptz(Guid clientUID, Guid cameraUid, int ptzNumber)
		{
			if (OnPtz != null)
				OnPtz(clientUID, cameraUid, ptzNumber);
		}

		public static void RviAlarm(Guid clientUID, string name)
		{
			if (OnRviAlarm != null)
				OnRviAlarm(clientUID, name);
		}

		public static void RviOpenWindow(Guid clientUid, string name, int x, int y, int monitorNumber, string login, string ip)
		{
			if (OnRviOpenWindow != null)
				OnRviOpenWindow(clientUid, name, x, y, monitorNumber,login,ip);
		}

		public static void ControlFireZone(Guid clientUID, Guid uid, ZoneCommandType commandType)
		{
			if (OnControlFireZone != null)
				OnControlFireZone(clientUID, uid, commandType);
		}
		public static void ControlGuardZone(Guid clientUID, Guid uid, GuardZoneCommandType commandType)
		{
			if (OnControlGuardZone != null)
				OnControlGuardZone(clientUID, uid, commandType);
		}
		public static void ControlDirection(Guid clientUID, Guid uid, DirectionCommandType commandType)
		{
			if (OnControlDirection != null)
				OnControlDirection(clientUID, uid, commandType);
		}
		public static void ControlGKDoor(Guid clientUID, Guid uid, GKDoorCommandType commandType)
		{
			if (OnControlGKDoor != null)
				OnControlGKDoor(clientUID, uid, commandType);
		}
		public static void ControlDelay(Guid clientUID, Guid uid, DelayCommandType commandType)
		{
			if (OnControlDelay != null)
				OnControlDelay(clientUID, uid, commandType);
		}

		public static void ControlPumpStation(Guid clientUID, Guid uid, PumpStationCommandType commandType)
		{
			if (OnControlPumpStation != null)
				OnControlPumpStation(clientUID, uid, commandType);
		}

		public static void ControlMPT(Guid clientUID, Guid uid, MPTCommandType commandType)
		{
			if (OnControlMPT != null)
				OnControlMPT(clientUID, uid, commandType);
		}

		public static void ExportJournal(Guid clientUID, bool isExportJournal, bool isExportPassJournal, DateTime minDate, DateTime maxDate, string path)
		{
			if (OnExportJournal != null)
				OnExportJournal(clientUID, isExportJournal, isExportPassJournal, minDate, maxDate, path);
		}
		public static void ExportOrganisation(Guid clientUID, bool isWithDeleted, Guid organisationUid, string path)
		{
			if (OnExportOrganisation != null)
				OnExportOrganisation(clientUID, isWithDeleted, organisationUid, path);
		}
		public static void ExportOrganisationList(Guid clientUID, bool isWithDeleted, string path)
		{
			if (OnExportOrganisationList != null)
				OnExportOrganisationList(clientUID, isWithDeleted, path);
		}
		public static void ExportConfiguration(Guid clientUID, bool isExportDevices, bool isExportDoors, bool isExportZones, string path)
		{
			if (OnExportConfiguration != null)
				OnExportConfiguration(clientUID, isExportDevices, isExportDoors, isExportZones, path);
		}
		public static void ImportOrganisation(Guid clientUID, bool isWithDeleted, string path)
		{
			if (OnImportOrganisation != null)
				OnImportOrganisation(clientUID, isWithDeleted, path);
		}
		public static void ImportOrganisationList(Guid clientUID, bool isWithDeleted, string path)
		{
			if (OnImportOrganisationList != null)
				OnImportOrganisationList(clientUID, isWithDeleted, path);
		}

		public static List<Organisation> GetOrganisations(Guid clientUID)
		{
			return OnGetOrganisations == null ? null : OnGetOrganisations(clientUID);
		}

		public delegate List<Organisation> GetOrganisationsEventHandler(Guid clientUID);
		public delegate SystemConfiguration GetSystemConfigurationDelegate();
	}
}
