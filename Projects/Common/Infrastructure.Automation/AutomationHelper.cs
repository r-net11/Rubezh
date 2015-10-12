using System;
using System.Collections.Generic;
using System.Linq;
using FiresecAPI.Automation;
using RubezhClient;
using System.Collections.ObjectModel;
using FiresecAPI;
using Property = FiresecAPI.Automation.Property;
using RubezhClient.SKDHelpers;
using FiresecAPI.GK;
using FiresecAPI.Models;
using FiresecAPI.Journal;
using System.Windows.Media;

namespace Infrastructure.Automation
{
	public static class AutomationHelper
    {
		public static List<Variable> GetAllVariables(Procedure procedure)
		{
			var allVariables = new List<Variable>(ProcedureExecutionContext.SystemConfiguration.AutomationConfiguration.GlobalVariables);
			allVariables.AddRange(procedure.Variables);
			allVariables.AddRange(procedure.Arguments);
			return allVariables;
		}

		public static List<Variable> GetAllVariables(Procedure procedure, ExplicitType explicitType, ObjectType objectType = ObjectType.Device, EnumType enumType = EnumType.DriverType)
		{
			var allVariables = GetAllVariables(procedure).FindAll(x => x.ExplicitType == explicitType);
			if (explicitType == ExplicitType.Enum)
				allVariables = allVariables.FindAll(x => x.EnumType == enumType);
			if (explicitType == ExplicitType.Object)
				allVariables = allVariables.FindAll(x => x.ObjectType == objectType);
			return allVariables;
		}

		public static List<Variable> GetAllVariables(Procedure procedure, ExplicitType ExplicitType, ObjectType objectType, bool isList)
		{
			return GetAllVariables(procedure).FindAll(x => x.ExplicitType == ExplicitType && x.ObjectType == objectType && x.IsList == isList);
		}

		public static List<Variable> GetAllVariables(List<Variable> allVariables, List<ExplicitType> explicitTypes, List<EnumType> enumTypes, List<ObjectType> objectTypes, bool? isList = null)
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
				return new List<Property> { Property.Description, Property.ShleifNo, Property.IntAddress, Property.State, Property.Type, Property.Uid };
			if (objectType == ObjectType.Zone)
				return new List<Property> { Property.Description, Property.No, Property.Uid, Property.Name, Property.State };
			if (objectType == ObjectType.Direction)
				return new List<Property> { Property.Description, Property.No, Property.Delay, Property.CurrentDelay, Property.Hold, Property.CurrentHold, Property.DelayRegime, Property.Uid, Property.Name, Property.State };
			if (objectType == ObjectType.Delay)
				return new List<Property> { Property.Description, Property.No, Property.Delay, Property.CurrentDelay, Property.Hold, Property.CurrentHold, Property.DelayRegime, Property.Uid, Property.Name, Property.State };
			if (objectType == ObjectType.GuardZone)
				return new List<Property> { Property.Description, Property.No, Property.Uid, Property.Name, Property.State };
			return new List<Property>();
		}

		public static List<ConditionType> ObjectTypeToConditionTypesList(ExplicitType ExplicitType)
		{
			if ((ExplicitType == ExplicitType.Integer) || (ExplicitType == ExplicitType.DateTime) || (ExplicitType == ExplicitType.Object) || ExplicitType == ExplicitType.Enum)
				return new List<ConditionType> { ConditionType.IsEqual, ConditionType.IsNotEqual, ConditionType.IsMore, ConditionType.IsNotMore, ConditionType.IsLess, ConditionType.IsNotLess };
			if ((ExplicitType == ExplicitType.Boolean) || (ExplicitType == ExplicitType.Object))
				return new List<ConditionType> { ConditionType.IsEqual, ConditionType.IsNotEqual };
			if (ExplicitType == ExplicitType.String)
				return new List<ConditionType> { ConditionType.IsEqual, ConditionType.IsNotEqual, ConditionType.StartsWith, ConditionType.EndsWith, ConditionType.Contains };
			return new List<ConditionType>();
		}

		public static ObservableCollection<T> GetEnumObs<T>()
		{
			return new ObservableCollection<T>(Enum.GetValues(typeof(T)).Cast<T>().ToList());
		}

		public static List<T> GetEnumList<T>()
		{
			return new List<T>(Enum.GetValues(typeof(T)).Cast<T>());
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
			var organisation = OrganisationHelper.GetSingle(uid);
			if (organisation != null)
				return organisation.Name;
			return "";
		}
    }
}
