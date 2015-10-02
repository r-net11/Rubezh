using FiresecAPI.AutomationCallback;
using FiresecAPI.Models;
using FiresecAPI.Automation;
using System;
using FiresecAPI.GK;
using FiresecAPI.Journal;
using System.Windows.Media;
using System.Linq;
using System.Collections.Generic;

namespace Infrastructure.Automation
{
	public static class ProcedureExecutionContext
	{
		public static ContextType ContextType { get; private set; }
		public static SystemConfiguration SystemConfiguration { get; private set; }
		public static SecurityConfiguration SecurityConfiguration { get; private set; }
		
		static event Action<AutomationCallbackResult, Guid?> OnSendCallback;
		static event Action<Guid, object> OnCallbackResponse;
		static event Action<Variable, ContextType> OnSynchronizeVariable;
		static event Action<string> OnAddJournalItem;
		static event Action<Guid, GKStateBit> OnControlGKDevice;
		static event Action<Guid, Guid?, Guid?, int> OnStartRecord;
		static event Action<Guid, Guid> OnStopRecord;
		static event Action<Guid, int> OnPtz;
		static event Action<string> OnRviAlarm;
		static event Action<Guid, ZoneCommandType> OnControlFireZone;
		static event Action<Guid, GuardZoneCommandType> OnControlGuardZone;
		static event Action<Guid, DirectionCommandType> OnControlDirection;
		static event Action<Guid, GKDoorCommandType> OnControlGKDoor;
		static event Action<Guid, DelayCommandType> OnControlDelay;
		static event Action<bool, bool, DateTime, DateTime, string> OnExportJournal;
		static event Action<bool, Guid, string> OnExportOrganisation;
		static event Action<bool, string> OnExportOrganisationList;
		static event Action<bool, bool, bool, string> OnExportConfiguration;
		static event Action<bool, string> OnImportOrganisation;
		static event Action<bool, string> OnImportOrganisationList;

		public static void Initialize(ContextType contextType, 
			SystemConfiguration systemConfiguration, 
			SecurityConfiguration securityConfiguration, 
			Action<AutomationCallbackResult, Guid?> onSendCallback = null,
			Action<Guid, object> onCallbackResponse = null,
			Action<Variable, ContextType> onSynchronizeVariable = null,
			Action<string> onAddJournalItem = null,
			Action<Guid, GKStateBit> onControlGKDevice = null,
			Action<Guid, Guid?, Guid?, int> onStartRecord = null,
			Action<Guid, Guid> onStopRecord = null,
			Action<Guid, int> onPtz = null,
			Action<string> onRviAlarm = null,
			Action<Guid, ZoneCommandType> onControlFireZone = null,
			Action<Guid, GuardZoneCommandType> onControlGuardZone = null,
			Action<Guid, DirectionCommandType> onControlDirection = null,
			Action<Guid, GKDoorCommandType> onControlGKDoor = null,
			Action<Guid, DelayCommandType> onControlDelay = null,
			Action<bool, bool, DateTime, DateTime, string> onExportJournal = null,
			Action<bool, Guid, string> onExportOrganisation = null,
			Action<bool, string> onExportOrganisationList = null,
			Action<bool, bool, bool, string> onExportConfiguration = null,
			Action<bool, string> onImportOrganisation = null,
			Action<bool, string> onImportOrganisationList = null
			)
		{
			ContextType = contextType;
			SystemConfiguration = systemConfiguration;
			SecurityConfiguration = securityConfiguration;
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
			OnExportJournal += onExportJournal;
			OnExportOrganisation += onExportOrganisation;
			OnExportOrganisationList += onExportOrganisationList;
			OnExportConfiguration += onExportConfiguration;
			OnImportOrganisation += onImportOrganisation;
			OnImportOrganisationList += onImportOrganisationList;

		}

		public static void SendCallback(AutomationCallbackResult callback, Guid? userUid = null)
		{
			if (OnSendCallback != null)
				OnSendCallback(callback, userUid);
		}

		public static void CallbackResponse(ContextType contextType, Guid callbackUid, object value)
		{
			if (contextType == ContextType)
			{
				ProcedureThread.CallbackResponse(callbackUid, value);
			}
			else
			{
				if (OnCallbackResponse != null)
					OnCallbackResponse(callbackUid, value);
			}
		}

		public static void AddJournalItem(string message)
		{
			if (OnAddJournalItem != null)
				OnAddJournalItem(message);
		}

		public static void ControlGKDevice(Guid deviceUid, GKStateBit command)
		{
			if (OnControlGKDevice != null)
				OnControlGKDevice(deviceUid, command);
		}

		public static void StartRecord(Guid cameraUid, Guid? journalItemUid, Guid? eventUid, int timeout)
		{
			if (OnStartRecord != null)
				OnStartRecord(cameraUid, journalItemUid, eventUid, timeout);
		}

		public static void StopRecord(Guid cameraUid, Guid eventUid)
		{
			if (OnStopRecord != null)
				OnStopRecord(cameraUid, eventUid);
		}

		public static void Ptz(Guid cameraUid, int ptzNumber)
		{
			if (OnPtz != null)
				OnPtz(cameraUid, ptzNumber);
		}

		public static void RviAlarm(string name)
		{
			if (OnRviAlarm != null)
				OnRviAlarm(name);
		}

		public static void ControlFireZone(Guid uid, ZoneCommandType commandType)
		{
			if (OnControlFireZone != null)
				OnControlFireZone(uid, commandType);
		}
		public static void ControlGuardZone(Guid uid, GuardZoneCommandType commandType)
		{
			if (OnControlGuardZone != null)
				OnControlGuardZone(uid, commandType);
		}
		public static void ControlDirection(Guid uid, DirectionCommandType commandType)
		{
			if (OnControlDirection != null)
				OnControlDirection(uid, commandType);
		}
		public static void ControlGKDoor(Guid uid, GKDoorCommandType commandType)
		{
			if (OnControlGKDoor != null)
				OnControlGKDoor(uid, commandType);
		}
		public static void ControlDelay(Guid uid, DelayCommandType commandType)
		{
			if (OnControlDelay != null)
				OnControlDelay(uid, commandType);
		}

		public static void ExportJournal(bool isExportJournal, bool isExportPassJournal, DateTime minDate, DateTime maxDate, string path)
		{
			if (OnExportJournal != null)
				OnExportJournal(isExportJournal, isExportPassJournal, minDate, maxDate, path);
		}
		public static void ExportOrganisation(bool isWithDeleted, Guid organisationUid, string path)
		{
			if (OnExportOrganisation != null)
				OnExportOrganisation(isWithDeleted, organisationUid, path);
		}
		public static void ExportOrganisationList(bool isWithDeleted, string path)
		{
			if (OnExportOrganisationList != null)
				OnExportOrganisationList(isWithDeleted, path);
		}
		public static void ExportConfiguration(bool isExportDevices, bool isExportDoors, bool isExportZones, string path)
		{
			if (OnExportConfiguration != null)
				OnExportConfiguration(isExportDevices, isExportDoors, isExportZones, path);
		}
		public static void ImportOrganisation(bool isWithDeleted, string path)
		{
			if (OnImportOrganisation != null)
				OnImportOrganisation(isWithDeleted, path);
		}
		public static void ImportOrganisationList(bool isWithDeleted, string path)
		{
			if (OnImportOrganisationList != null)
				OnImportOrganisationList(isWithDeleted, path);
		}

		public static void SynchronizeVariable(Variable variable, ContextType targetContextType)
		{
			if (variable.IsGlobal && variable.ContextType == ContextType.Server && OnSynchronizeVariable != null)
				OnSynchronizeVariable(variable, targetContextType);
		}
		
		public static object GetVariableValue(Variable source)
		{
			if (source == null)
				return null;

			SynchronizeVariable(source, ContextType.Client);
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
		
		public static void SetVariableValue(Variable target, object value)
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
			SynchronizeVariable(target, ContextType.Server);
		}
	}
}
