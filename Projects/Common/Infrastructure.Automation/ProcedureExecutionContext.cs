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
		static event Action<Guid, object, Guid> OnCallbackResponse;
		static event Action<Variable, ContextType, Guid> OnSynchronizeVariable;
		static event Action<string, Guid> OnAddJournalItem;
		static event Action<Guid, GKStateBit, Guid> OnControlGKDevice;
		static event Action<Guid, Guid?, Guid?, int, Guid> OnStartRecord;
		static event Action<Guid, Guid, Guid> OnStopRecord;
		static event Action<Guid, int, Guid> OnPtz;
		static event Action<string, Guid> OnRviAlarm;
		static event Action<Guid, ZoneCommandType, Guid> OnControlFireZone;
		static event Action<Guid, GuardZoneCommandType, Guid> OnControlGuardZone;
		static event Action<Guid, DirectionCommandType, Guid> OnControlDirection;
		static event Action<Guid, GKDoorCommandType, Guid> OnControlGKDoor;
		static event Action<Guid, DelayCommandType, Guid> OnControlDelay;
		static event Action<Guid, PumpStationCommandType, Guid> OnControlPumpStation;
		static event Action<Guid, MPTCommandType, Guid> OnControlMPT;
		static event Action<bool, bool, DateTime, DateTime, string, Guid> OnExportJournal;
		static event Action<bool, Guid, string, Guid> OnExportOrganisation;
		static event Action<bool, string, Guid> OnExportOrganisationList;
		static event Action<bool, bool, bool, string, Guid> OnExportConfiguration;
		static event Action<bool, string, Guid> OnImportOrganisation;
		static event Action<bool, string, Guid> OnImportOrganisationList;
		static GetOrganisationsEventHandler OnGetOrganisations;

		public static void Initialize(ContextType contextType,
			SystemConfiguration systemConfiguration,
			SecurityConfiguration securityConfiguration,
			Action<AutomationCallbackResult, Guid?> onSendCallback = null,
			Action<Guid, object, Guid> onCallbackResponse = null,
			Action<Variable, ContextType, Guid> onSynchronizeVariable = null,
			Action<string, Guid> onAddJournalItem = null,
			Action<Guid, GKStateBit, Guid> onControlGKDevice = null,
			Action<Guid, Guid?, Guid?, int, Guid> onStartRecord = null,
			Action<Guid, Guid, Guid> onStopRecord = null,
			Action<Guid, int, Guid> onPtz = null,
			Action<string, Guid> onRviAlarm = null,
			Action<Guid, ZoneCommandType, Guid> onControlFireZone = null,
			Action<Guid, GuardZoneCommandType, Guid> onControlGuardZone = null,
			Action<Guid, DirectionCommandType, Guid> onControlDirection = null,
			Action<Guid, GKDoorCommandType, Guid> onControlGKDoor = null,
			Action<Guid, DelayCommandType, Guid> onControlDelay = null,
			Action<Guid, PumpStationCommandType, Guid> onControlPumpStation = null,
			Action<Guid, MPTCommandType, Guid> onControlMPT = null,
			Action<bool, bool, DateTime, DateTime, string, Guid> onExportJournal = null,
			Action<bool, Guid, string, Guid> onExportOrganisation = null,
			Action<bool, string, Guid> onExportOrganisationList = null,
			Action<bool, bool, bool, string, Guid> onExportConfiguration = null,
			Action<bool, string, Guid> onImportOrganisation = null,
			Action<bool, string, Guid> onImportOrganisationList = null,
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

		public static void SendCallback(AutomationCallbackResult callback, Guid? userUid = null)
		{
			if (OnSendCallback != null)
				OnSendCallback(callback, userUid);
		}

		public static void CallbackResponse(ContextType contextType, Guid callbackUid, object value, Guid clientUID)
		{
			if (contextType == ContextType)
			{
				ProcedureThread.CallbackResponse(callbackUid, value);
			}
			else
			{
				if (OnCallbackResponse != null)
					OnCallbackResponse(callbackUid, value, clientUID);
			}
		}

		public static void AddJournalItem(string message, Guid clientUID)
		{
			if (OnAddJournalItem != null)
				OnAddJournalItem(message, clientUID);
		}

		public static void ControlGKDevice(Guid deviceUid, GKStateBit command, Guid clientUID)
		{
			if (OnControlGKDevice != null)
				OnControlGKDevice(deviceUid, command, clientUID);
		}

		public static void StartRecord(Guid cameraUid, Guid? journalItemUid, Guid? eventUid, int timeout, Guid clientUID)
		{
			if (OnStartRecord != null)
				OnStartRecord(cameraUid, journalItemUid, eventUid, timeout, clientUID);
		}

		public static void StopRecord(Guid cameraUid, Guid eventUid, Guid clientUID)
		{
			if (OnStopRecord != null)
				OnStopRecord(cameraUid, eventUid, clientUID);
		}

		public static void Ptz(Guid cameraUid, int ptzNumber, Guid clientUID)
		{
			if (OnPtz != null)
				OnPtz(cameraUid, ptzNumber, clientUID);
		}

		public static void RviAlarm(string name, Guid clientUID)
		{
			if (OnRviAlarm != null)
				OnRviAlarm(name, clientUID);
		}

		public static void ControlFireZone(Guid uid, ZoneCommandType commandType, Guid clientUID)
		{
			if (OnControlFireZone != null)
				OnControlFireZone(uid, commandType, clientUID);
		}
		public static void ControlGuardZone(Guid uid, GuardZoneCommandType commandType, Guid clientUID)
		{
			if (OnControlGuardZone != null)
				OnControlGuardZone(uid, commandType, clientUID);
		}
		public static void ControlDirection(Guid uid, DirectionCommandType commandType, Guid clientUID)
		{
			if (OnControlDirection != null)
				OnControlDirection(uid, commandType, clientUID);
		}
		public static void ControlGKDoor(Guid uid, GKDoorCommandType commandType, Guid clientUID)
		{
			if (OnControlGKDoor != null)
				OnControlGKDoor(uid, commandType, clientUID);
		}
		public static void ControlDelay(Guid uid, DelayCommandType commandType, Guid clientUID)
		{
			if (OnControlDelay != null)
				OnControlDelay(uid, commandType, clientUID);
		}

		public static void ControlPumpStation(Guid uid, PumpStationCommandType commandType, Guid clientUID)
		{
			if (OnControlPumpStation != null)
				OnControlPumpStation(uid, commandType, clientUID);
		}

		public static void ControlMPT(Guid uid, MPTCommandType commandType, Guid clientUID)
		{
			if (OnControlMPT != null)
				OnControlMPT(uid, commandType, clientUID);
		}

		public static void ExportJournal(bool isExportJournal, bool isExportPassJournal, DateTime minDate, DateTime maxDate, string path, Guid clientUID)
		{
			if (OnExportJournal != null)
				OnExportJournal(isExportJournal, isExportPassJournal, minDate, maxDate, path, clientUID);
		}
		public static void ExportOrganisation(bool isWithDeleted, Guid organisationUid, string path, Guid clientUID)
		{
			if (OnExportOrganisation != null)
				OnExportOrganisation(isWithDeleted, organisationUid, path, clientUID);
		}
		public static void ExportOrganisationList(bool isWithDeleted, string path, Guid clientUID)
		{
			if (OnExportOrganisationList != null)
				OnExportOrganisationList(isWithDeleted, path, clientUID);
		}
		public static void ExportConfiguration(bool isExportDevices, bool isExportDoors, bool isExportZones, string path, Guid clientUID)
		{
			if (OnExportConfiguration != null)
				OnExportConfiguration(isExportDevices, isExportDoors, isExportZones, path, clientUID);
		}
		public static void ImportOrganisation(bool isWithDeleted, string path, Guid clientUID)
		{
			if (OnImportOrganisation != null)
				OnImportOrganisation(isWithDeleted, path, clientUID);
		}
		public static void ImportOrganisationList(bool isWithDeleted, string path, Guid clientUID)
		{
			if (OnImportOrganisationList != null)
				OnImportOrganisationList(isWithDeleted, path, clientUID);
		}

		public static void SynchronizeVariable(Variable variable, ContextType targetContextType, Guid clientUID)
		{
			if (variable.IsGlobal && variable.ContextType == ContextType.Server && OnSynchronizeVariable != null)
				OnSynchronizeVariable(variable, targetContextType, clientUID);
		}

		public static object GetVariableValue(Variable source, Guid clientUID)
		{
			if (source == null)
				return null;

			SynchronizeVariable(source, ContextType.Client, clientUID);
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

		public static void SetVariableValue(Variable target, object value, Guid clientUID)
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
			SynchronizeVariable(target, ContextType.Server, clientUID);
		}

		public static List<Organisation> GetOrganisations(Guid clientUID)
		{
			return OnGetOrganisations == null ? null : OnGetOrganisations(clientUID);
		}
	}

	public delegate List<Organisation> GetOrganisationsEventHandler(Guid clientUID);
}
