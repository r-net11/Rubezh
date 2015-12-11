using RubezhAPI.Automation;
using RubezhAPI.AutomationCallback;
using RubezhAPI.GK;
using RubezhAPI.Journal;
using RubezhAPI.Models;
using RubezhAPI.SKD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace Infrastructure.Automation
{
	public static class ProcedureExecutionContext
	{
		public static ContextType ContextType { get; private set; }
		public static SystemConfiguration SystemConfiguration { get; private set; }
		public static SecurityConfiguration SecurityConfiguration { get; private set; }

		static event Action<AutomationCallbackResult, Guid?> OnSendCallback;
		static event Action<Guid, Guid, object> OnCallbackResponse;
		static event Action<Guid, Variable, ContextType> OnSynchronizeVariable;
		static event Action<Guid, string> OnAddJournalItem;
		static event Action<Guid, Guid, GKStateBit> OnControlGKDevice;
		static event Action<Guid, Guid, Guid?, Guid?, int> OnStartRecord;
		static event Action<Guid, Guid, Guid> OnStopRecord;
		static event Action<Guid, Guid, int> OnPtz;
		static event Action<Guid, string> OnRviAlarm;
		static event Action<Guid, Guid, ZoneCommandType> OnControlFireZone;
		static event Action<Guid, Guid, GuardZoneCommandType> OnControlGuardZone;
		static event Action<Guid, Guid, DirectionCommandType> OnControlDirection;
		static event Action<Guid, Guid, GKDoorCommandType> OnControlGKDoor;
		static event Action<Guid, Guid, DelayCommandType> OnControlDelay;
		static event Action<Guid, Guid, PumpStationCommandType> OnControlPumpStation;
		static event Action<Guid, Guid, MPTCommandType> OnControlMPT;
		static event Action<Guid, bool, bool, DateTime, DateTime, string> OnExportJournal;
		static event Action<Guid, bool, Guid, string> OnExportOrganisation;
		static event Action<Guid, bool, string> OnExportOrganisationList;
		static event Action<Guid, bool, bool, bool, string> OnExportConfiguration;
		static event Action<Guid, bool, string> OnImportOrganisation;
		static event Action<Guid, bool, string> OnImportOrganisationList;
		static GetOrganisationsEventHandler OnGetOrganisations;

		public static void Initialize(ContextType contextType,
			SystemConfiguration systemConfiguration,
			SecurityConfiguration securityConfiguration,
			Action<AutomationCallbackResult, Guid?> onSendCallback = null,
			Action<Guid, Guid, object> onCallbackResponse = null,
			Action<Guid, Variable, ContextType> onSynchronizeVariable = null,
			Action<Guid, string> onAddJournalItem = null,
			Action<Guid, Guid, GKStateBit> onControlGKDevice = null,
			Action<Guid, Guid, Guid?, Guid?, int> onStartRecord = null,
			Action<Guid, Guid, Guid> onStopRecord = null,
			Action<Guid, Guid, int> onPtz = null,
			Action<Guid, string> onRviAlarm = null,
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
			UpdateConfiguration(systemConfiguration, securityConfiguration);
			ContextType = contextType;
			OnSendCallback += onSendCallback;
			OnCallbackResponse += onCallbackResponse;
			OnSynchronizeVariable += onSynchronizeVariable;
			OnAddJournalItem += onAddJournalItem;
			OnControlGKDevice += onControlGKDevice;
			OnStartRecord += onStartRecord;
			OnStopRecord += onStopRecord;
			OnPtz += onPtz;
			OnRviAlarm += onRviAlarm;
			OnControlFireZone += onControlFireZone;
			OnControlGuardZone += onControlGuardZone;
			OnControlDirection += onControlDirection;
			OnControlGKDoor += onControlGKDoor;
			OnControlDelay += onControlDelay;
			OnControlPumpStation += onControlPumpStation;
			OnControlMPT += onControlMPT;
			OnExportJournal += onExportJournal;
			OnExportOrganisation += onExportOrganisation;
			OnExportOrganisationList += onExportOrganisationList;
			OnExportConfiguration += onExportConfiguration;
			OnImportOrganisation += onImportOrganisation;
			OnImportOrganisationList += onImportOrganisationList;
			OnGetOrganisations = onGetOrganisations;
		}

		public static void UpdateConfiguration(SystemConfiguration systemConfiguration, SecurityConfiguration securityConfiguration)
		{
			SystemConfiguration = systemConfiguration;
			SecurityConfiguration = securityConfiguration;
		}

		public static void SendCallback(AutomationCallbackResult callback, Guid? clientUID)
		{
			if (OnSendCallback != null)
				OnSendCallback(callback, clientUID);
		}

		public static void CallbackResponse(Guid clientUID, ContextType contextType, Guid callbackUid, object value)
		{
			if (contextType == ContextType)
			{
				ProcedureThread.CallbackResponse(callbackUid, value);
			}
			else
			{
				if (OnCallbackResponse != null)
					OnCallbackResponse(clientUID, callbackUid, value);
			}
		}

		public static void AddJournalItem(Guid clientUID, string message)
		{
			if (OnAddJournalItem != null)
				OnAddJournalItem(clientUID, message);
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

		public static void SynchronizeVariable(Guid clientUID, Variable variable, ContextType targetContextType)
		{
			if (variable.IsGlobal && variable.ContextType == ContextType.Server && OnSynchronizeVariable != null)
				OnSynchronizeVariable(clientUID, variable, targetContextType);
		}

		public static object GetVariableValue(Guid clientUID, Variable source)
		{
			if (source == null)
				return null;

			SynchronizeVariable(clientUID, source, ContextType.Client);
			return GetValue(source.ExplicitValue, source.ExplicitType, source.EnumType);
		}

		public static object GetValue(Variable variable)
		{
			if (variable == null)
				return new object();

			return variable.IsList ?
				GetListValue(variable.ExplicitValues, variable.ExplicitType, variable.EnumType) :
				GetValue(variable.ExplicitValue, variable.ExplicitType, variable.EnumType);
		}

		public static object GetValue(ExplicitValue explicitValue, ExplicitType explicitType, EnumType enumType)
		{
			switch (explicitType)
			{
				case ExplicitType.Boolean:
					return explicitValue.BoolValue;
				case ExplicitType.DateTime:
					return explicitValue.DateTimeValue;
				case ExplicitType.Integer:
					return explicitValue.IntValue;
				case ExplicitType.String:
					return explicitValue.StringValue;
				case ExplicitType.Object:
					return explicitValue.UidValue;
				case ExplicitType.Enum:
					switch (enumType)
					{
						case EnumType.DriverType:
							return explicitValue.DriverTypeValue;
						case EnumType.StateType:
							return explicitValue.StateTypeValue;
						case EnumType.PermissionType:
							return explicitValue.PermissionTypeValue;
						case EnumType.JournalEventNameType:
							return explicitValue.JournalEventNameTypeValue;
						case EnumType.JournalEventDescriptionType:
							return explicitValue.JournalEventDescriptionTypeValue;
						case EnumType.JournalObjectType:
							return explicitValue.JournalObjectTypeValue;
						case EnumType.ColorType:
							return explicitValue.ColorValue.ToString();
					}
					break;
			}
			return new object();
		}

		public static object[] GetListValue(IEnumerable<ExplicitValue> explicitValues, ExplicitType explicitType, EnumType enumType)
		{
			switch (explicitType)
			{
				case ExplicitType.Boolean:
					return explicitValues.Select(x => (object)x.BoolValue).ToArray();
				case ExplicitType.DateTime:
					return explicitValues.Select(x => (object)x.DateTimeValue).ToArray();
				case ExplicitType.Integer:
					return explicitValues.Select(x => (object)x.IntValue).ToArray();
				case ExplicitType.String:
					return explicitValues.Select(x => (object)x.StringValue).ToArray();
				case ExplicitType.Object:
					return explicitValues.Select(x => (object)x.UidValue).ToArray();
				case ExplicitType.Enum:
					switch (enumType)
					{
						case EnumType.DriverType:
							return explicitValues.Select(x => (object)x.DriverTypeValue).ToArray();
						case EnumType.StateType:
							return explicitValues.Select(x => (object)x.StateTypeValue).ToArray();
						case EnumType.PermissionType:
							return explicitValues.Select(x => (object)x.PermissionTypeValue).ToArray();
						case EnumType.JournalEventNameType:
							return explicitValues.Select(x => (object)x.JournalEventNameTypeValue).ToArray();
						case EnumType.JournalEventDescriptionType:
							return explicitValues.Select(x => (object)x.JournalEventDescriptionTypeValue).ToArray();
						case EnumType.JournalObjectType:
							return explicitValues.Select(x => (object)x.JournalObjectTypeValue).ToArray();
						case EnumType.ColorType:
							return explicitValues.Select(x => (object)x.ColorValue.ToString()).ToArray();
					}
					break;
			}
			return new object[0];
		}

		public static void SetVariableValue(Guid clientUID, Variable target, object value)
		{
			if (target == null)
				return;

			if (value is IEnumerable<object>)
			{
				var listValue = value as IEnumerable<object>;

				if (target.ExplicitType == ExplicitType.Integer)
					target.ExplicitValues = listValue.Select(x => new ExplicitValue() { IntValue = Convert.ToInt32(x) }).ToList();
				if (target.ExplicitType == ExplicitType.String)
					target.ExplicitValues = listValue.Select(x => new ExplicitValue() { StringValue = Convert.ToString(x) }).ToList();
				if (target.ExplicitType == ExplicitType.Boolean)
					target.ExplicitValues = listValue.Select(x => new ExplicitValue() { BoolValue = Convert.ToBoolean(x) }).ToList();
				if (target.ExplicitType == ExplicitType.DateTime)
					target.ExplicitValues = listValue.Select(x => new ExplicitValue() { DateTimeValue = Convert.ToDateTime(x) }).ToList();
				if (target.ExplicitType == ExplicitType.Object)
					target.ExplicitValues = listValue.Select(x => new ExplicitValue() { UidValue = (Guid)x }).ToList();
				if (target.ExplicitType == ExplicitType.Enum)
				{
					if (target.EnumType == EnumType.DriverType)
						target.ExplicitValues = listValue.Select(x => new ExplicitValue() { DriverTypeValue = (GKDriverType)x }).ToList();
					if (target.EnumType == EnumType.StateType)
						target.ExplicitValues = listValue.Select(x => new ExplicitValue() { StateTypeValue = (XStateClass)x }).ToList();
					if (target.EnumType == EnumType.PermissionType)
						target.ExplicitValues = listValue.Select(x => new ExplicitValue() { PermissionTypeValue = (PermissionType)x }).ToList();
					if (target.EnumType == EnumType.JournalEventNameType)
						target.ExplicitValues = listValue.Select(x => new ExplicitValue() { JournalEventNameTypeValue = (JournalEventNameType)x }).ToList();
					if (target.EnumType == EnumType.JournalEventDescriptionType)
						target.ExplicitValues = listValue.Select(x => new ExplicitValue() { JournalEventDescriptionTypeValue = (JournalEventDescriptionType)x }).ToList();
					if (target.EnumType == EnumType.JournalObjectType)
						target.ExplicitValues = listValue.Select(x => new ExplicitValue() { JournalObjectTypeValue = (JournalObjectType)x }).ToList();
					if (target.EnumType == EnumType.ColorType)
						target.ExplicitValues = listValue.Select(x => new ExplicitValue() { ColorValue = (Color)x }).ToList();
				}
			}
			else
			{
				if (target.ExplicitType == ExplicitType.Integer)
					target.ExplicitValue.IntValue = Convert.ToInt32(value);
				if (target.ExplicitType == ExplicitType.String)
					target.ExplicitValue.StringValue = Convert.ToString(value);
				if (target.ExplicitType == ExplicitType.Boolean)
					target.ExplicitValue.BoolValue = Convert.ToBoolean(value);
				if (target.ExplicitType == ExplicitType.DateTime)
					target.ExplicitValue.DateTimeValue = Convert.ToDateTime(value);
				if (target.ExplicitType == ExplicitType.Object)
					target.ExplicitValue.UidValue = (Guid)value;
				if (target.ExplicitType == ExplicitType.Enum)
				{
					if (target.EnumType == EnumType.DriverType)
						target.ExplicitValue.DriverTypeValue = (GKDriverType)value;
					if (target.EnumType == EnumType.StateType)
						target.ExplicitValue.StateTypeValue = (XStateClass)value;
					if (target.EnumType == EnumType.PermissionType)
						target.ExplicitValue.PermissionTypeValue = (PermissionType)value;
					if (target.EnumType == EnumType.JournalEventNameType)
						target.ExplicitValue.JournalEventNameTypeValue = (JournalEventNameType)value;
					if (target.EnumType == EnumType.JournalEventDescriptionType)
						target.ExplicitValue.JournalEventDescriptionTypeValue = (JournalEventDescriptionType)value;
					if (target.EnumType == EnumType.JournalObjectType)
						target.ExplicitValue.JournalObjectTypeValue = (JournalObjectType)value;
					if (target.EnumType == EnumType.ColorType)
						target.ExplicitValue.ColorValue = (Color)value;
				}
			}
			SynchronizeVariable(clientUID, target, ContextType.Server);
		}

		public static List<Organisation> GetOrganisations(Guid clientUID)
		{
			return OnGetOrganisations == null ? null : OnGetOrganisations(clientUID);
		}
	}

	public delegate List<Organisation> GetOrganisationsEventHandler(Guid clientUID);
}
