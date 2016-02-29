using RubezhAPI;
using RubezhAPI.Automation;
using RubezhAPI.AutomationCallback;
using RubezhClient;
using RubezhClient.SKDHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Property = RubezhAPI.Automation.Property;

namespace Infrastructure.Automation
{
	public static class AutomationHelper
	{
		public static bool CheckLayoutFilter(AutomationCallbackResult automationCallbackResult, Guid? layoutUID)
		{
			if (automationCallbackResult == null || automationCallbackResult.Data == null || !(automationCallbackResult.Data is UIAutomationCallbackData))
				return true;
			var data = automationCallbackResult.Data as UIAutomationCallbackData;
			return data.LayoutFilter != null && layoutUID.HasValue && data.LayoutFilter.Contains(layoutUID.Value);
		}

		public static List<Variable> GetAllVariables(Procedure procedure)
		{
			var allVariables =
				new List<Variable>(ProcedureExecutionContext.SystemConfiguration.AutomationConfiguration.GlobalVariables);
			allVariables.AddRange(GetLocalVariables(procedure));
			return allVariables;
		}

		public static List<Variable> GetLocalVariables(Procedure procedure)
		{
			var localVariables = new List<Variable>();
			if (procedure != null)
			{
				localVariables.AddRange(procedure.Variables);
				localVariables.AddRange(procedure.Arguments);
			}
			return localVariables;
		}

		public static List<Variable> GetAllVariables(Procedure procedure, ExplicitType explicitType,
			ObjectType objectType = ObjectType.Device, EnumType enumType = EnumType.DriverType)
		{
			var allVariables = GetAllVariables(procedure).FindAll(x => x.ExplicitType == explicitType);
			if (explicitType == ExplicitType.Enum)
				allVariables = allVariables.FindAll(x => x.EnumType == enumType);
			if (explicitType == ExplicitType.Object)
				allVariables = allVariables.FindAll(x => x.ObjectType == objectType);
			return allVariables;
		}

		public static List<Variable> GetAllVariables(Procedure procedure, ExplicitType ExplicitType, ObjectType objectType,
			bool isList)
		{
			return
				GetAllVariables(procedure)
					.FindAll(x => x.ExplicitType == ExplicitType && x.ObjectType == objectType && x.IsList == isList);
		}

		public static List<Variable> GetAllVariables(List<Variable> allVariables, List<ExplicitType> explicitTypes,
			List<EnumType> enumTypes, List<ObjectType> objectTypes, bool? isList = null)
		{
			var variables = new List<Variable>(allVariables);
			if (explicitTypes != null)
			{
				variables = variables.FindAll(x => explicitTypes.Contains(x.ExplicitType));
				if (explicitTypes.Contains(ExplicitType.Enum))
				{
					variables = variables.FindAll(x => enumTypes.Contains(x.EnumType));
				}
				if (explicitTypes.Contains(ExplicitType.Object))
				{
					variables = variables.FindAll(x => objectTypes.Contains(x.ObjectType));
				}
			}
			if (isList != null)
			{
				variables = variables.FindAll(x => x.IsList == isList);
			}
			return variables;
		}

		public static List<Property> ObjectTypeToProperiesList(ObjectType objectType)
		{
			if (objectType == ObjectType.Device)
				return new List<Property>
				{
					Property.Description,
					Property.ShleifNo,
					Property.IntAddress,
					Property.State,
					Property.Type,
					Property.Uid
				};
			if (objectType == ObjectType.Zone)
				return new List<Property> { Property.Description, Property.No, Property.Uid, Property.Name, Property.State };
			if (objectType == ObjectType.Direction)
				return new List<Property>
				{
					Property.Description,
					Property.No,
					Property.Delay,
					Property.CurrentDelay,
					Property.Hold,
					Property.CurrentHold,
					Property.DelayRegime,
					Property.Uid,
					Property.Name,
					Property.State
				};
			if (objectType == ObjectType.Delay)
				return new List<Property>
				{
					Property.Description,
					Property.No,
					Property.Delay,
					Property.CurrentDelay,
					Property.Hold,
					Property.CurrentHold,
					Property.DelayRegime,
					Property.Uid,
					Property.Name,
					Property.State
				};
			if (objectType == ObjectType.GuardZone)
				return new List<Property> { Property.Description, Property.No, Property.Uid, Property.Name, Property.State };
			if (objectType == ObjectType.GKDoor)
				return new List<Property>
				{
					Property.Description,
					Property.No,
					Property.Delay,
					Property.CurrentDelay,
					Property.Hold,
					Property.CurrentHold,
					Property.Uid,
					Property.Name,
					Property.State
				};
			if (objectType == ObjectType.Organisation)
				return new List<Property> { Property.Description, Property.Uid, Property.Name };
			if (objectType == ObjectType.VideoDevice)
				return new List<Property> { Property.Uid, Property.Name };
			if (objectType == ObjectType.PumpStation)
				return new List<Property> { Property.Uid, Property.No, Property.Delay, Property.Name, Property.State };
			if (objectType == ObjectType.MPT)
				return new List<Property> { Property.Uid, Property.Description, Property.Name, Property.State };
			return new List<Property>();
		}

		public static List<ConditionType> ObjectTypeToConditionTypesList(ExplicitType ExplicitType)
		{
			if (ExplicitType == ExplicitType.Integer || ExplicitType == ExplicitType.Float ||
				ExplicitType == ExplicitType.DateTime || ExplicitType == ExplicitType.Enum)
				return new List<ConditionType>
				{
					ConditionType.IsEqual,
					ConditionType.IsNotEqual,
					ConditionType.IsMore,
					ConditionType.IsNotMore,
					ConditionType.IsLess,
					ConditionType.IsNotLess
				};
			if ((ExplicitType == ExplicitType.Boolean) || (ExplicitType == ExplicitType.Object))
				return new List<ConditionType> { ConditionType.IsEqual, ConditionType.IsNotEqual };
			if (ExplicitType == ExplicitType.String)
				return new List<ConditionType>
				{
					ConditionType.IsEqual,
					ConditionType.IsNotEqual,
					ConditionType.StartsWith,
					ConditionType.EndsWith,
					ConditionType.Contains
				};
			return new List<ConditionType>();
		}

		public static ObservableCollection<T> GetEnumObs<T>()
		{
			return new ObservableCollection<T>(Enum.GetValues(typeof(T)).Cast<T>().ToList());
		}

		public static List<T> GetEnumList<T>()
		{
			var result = new List<T>(Enum.GetValues(typeof(T)).Cast<T>());
			result.Sort();
			return result;
		}

		public static string GetProcedureName(Guid procedureUid)
		{
			var procedure = ClientManager.SystemConfiguration.AutomationConfiguration.Procedures.FirstOrDefault(x => x.Uid == procedureUid);
			return procedure == null ? "" : procedure.Name;
		}

		public static string GetStringValue(object obj)
		{
			if (obj == null)
				return "";

			var objType = obj.GetType();
			if (objType == typeof(bool))
				return (bool)obj ? "Да" : "Нет";

			if (objType.IsEnum)
				return ((Enum)obj).ToDescription();

			if (objType == typeof(Guid))
				return UidToObjectName((Guid)obj);

			return obj.ToString();
		}

		public static string GetStringValue(ExplicitValue explicitValue, ExplicitType explicitType, EnumType enumType)
		{
			switch (explicitType)
			{
				case ExplicitType.Boolean:
					return explicitValue.BoolValue ? "Да" : "Нет";
				case ExplicitType.DateTime:
					return explicitValue.DateTimeValue.ToString();
				case ExplicitType.Integer:
					return explicitValue.IntValue.ToString();
				case ExplicitType.Float:
					return explicitValue.FloatValue.ToString();
				case ExplicitType.String:
					return explicitValue.StringValue;
				case ExplicitType.Enum:
					{
						switch (enumType)
						{
							case EnumType.StateType:
								return explicitValue.StateTypeValue.ToDescription();
							case EnumType.DriverType:
								return explicitValue.DriverTypeValue.ToDescription();
							case EnumType.PermissionType:
								return explicitValue.PermissionTypeValue.ToDescription();
							case EnumType.JournalEventDescriptionType:
								return explicitValue.JournalEventDescriptionTypeValue.ToDescription();
							case EnumType.JournalEventNameType:
								return explicitValue.JournalEventNameTypeValue.ToDescription();
							case EnumType.JournalObjectType:
								return explicitValue.JournalObjectTypeValue.ToDescription();
							case EnumType.ColorType:
								return explicitValue.ColorValue.ToString();
						}
					}
					break;
				case ExplicitType.Object:
				default:
					return UidToObjectName(explicitValue.UidValue);
			}
			return "";
		}

		static string UidToObjectName(Guid uid)
		{
			if (uid == Guid.Empty)
				return "";
			var device = GKManager.DeviceConfiguration.Devices.FirstOrDefault(x => x.UID == uid);
			if (device != null)
				return device.PresentationName;
			var zone = GKManager.DeviceConfiguration.Zones.FirstOrDefault(x => x.UID == uid);
			if (zone != null)
				return zone.PresentationName;
			var guardZone = GKManager.DeviceConfiguration.GuardZones.FirstOrDefault(x => x.UID == uid);
			if (guardZone != null)
				return guardZone.PresentationName;
			var camera = ProcedureExecutionContext.SystemConfiguration.Cameras.FirstOrDefault(x => x.UID == uid);
			if (camera != null)
				return camera.PresentationName;
			var gKDoor = GKManager.Doors.FirstOrDefault(x => x.UID == uid);
			if (gKDoor != null)
				return gKDoor.PresentationName;
			var direction = GKManager.DeviceConfiguration.Directions.FirstOrDefault(x => x.UID == uid);
			if (direction != null)
				return direction.PresentationName;
			var delay = GKManager.DeviceConfiguration.Delays.FirstOrDefault(x => x.UID == uid);
			if (delay != null)
				return delay.PresentationName;
			var pumpStation = GKManager.DeviceConfiguration.PumpStations.FirstOrDefault(x => x.UID == uid);
			if (pumpStation != null)
				return pumpStation.PresentationName;
			var mpt = GKManager.DeviceConfiguration.MPTs.FirstOrDefault(x => x.UID == uid);
			if (mpt != null)
				return mpt.PresentationName;
			var organisation = OrganisationHelper.GetSingle(uid);
			if (organisation != null)
				return organisation.Name;
			return "";
		}
	}
}
