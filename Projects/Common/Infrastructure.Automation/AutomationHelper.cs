using RubezhAPI.Automation;
using RubezhAPI.AutomationCallback;
using RubezhAPI.GK;
using RubezhAPI.Models;
using RubezhAPI.SKD;
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
				return new List<Property> { Property.Description, Property.Uid, Property.No, Property.Delay, Property.Name, Property.State };
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

		public static ObjectReference GetObjectReference(object obj)
		{
			if (obj == null)
				return new ObjectReference();
			if (obj is ObjectReference)
				return (ObjectReference)obj;
			if (obj is GKDevice)
				return new ObjectReference { ObjectType = ObjectType.Device, UID = ((ModelBase)obj).UID };
			if (obj is GKZone)
				return new ObjectReference { ObjectType = ObjectType.Zone, UID = ((ModelBase)obj).UID };
			if (obj is GKGuardZone)
				return new ObjectReference { ObjectType = ObjectType.GuardZone, UID = ((ModelBase)obj).UID };
			if (obj is Camera)
				return new ObjectReference { ObjectType = ObjectType.VideoDevice, UID = ((ModelBase)obj).UID };
			if (obj is GKDoor)
				return new ObjectReference { ObjectType = ObjectType.GKDoor, UID = ((ModelBase)obj).UID };
			if (obj is GKDirection)
				return new ObjectReference { ObjectType = ObjectType.Direction, UID = ((ModelBase)obj).UID };
			if (obj is GKDelay)
				return new ObjectReference { ObjectType = ObjectType.Delay, UID = ((ModelBase)obj).UID };
			if (obj is GKPumpStation)
				return new ObjectReference { ObjectType = ObjectType.PumpStation, UID = ((ModelBase)obj).UID };
			if (obj is GKMPT)
				return new ObjectReference { ObjectType = ObjectType.MPT, UID = ((ModelBase)obj).UID };
			if (obj is Organisation)
				return new ObjectReference { ObjectType = ObjectType.Device, UID = ((SKDModelBase)obj).UID };

			return new ObjectReference();
		}
	}
}
