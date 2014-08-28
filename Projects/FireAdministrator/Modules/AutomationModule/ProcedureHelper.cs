using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Automation;
using FiresecClient;
using FiresecAPI.GK;
using ValueType = FiresecAPI.Automation.ValueType;
using System.Collections.ObjectModel;

namespace AutomationModule
{
	public static class ProcedureHelper
	{
		public static List<Variable> GetAllVariables(Procedure procedure)
		{
			var allVariables = new List<Variable>(FiresecManager.SystemConfiguration.AutomationConfiguration.GlobalVariables);
			allVariables.AddRange(procedure.Variables);
			allVariables.AddRange(procedure.Arguments);
			return allVariables;
		}

		public static List<Variable> GetAllVariables(Procedure procedure, ValueType valueType, ObjectType objectType = ObjectType.Device, bool isList = false)
		{
			return GetAllVariables(procedure).FindAll(x => x.ValueType == valueType && x.ObjectType == objectType && x.IsList == isList);
		}

		public static List<Property> ObjectTypeToProperiesList(ObjectType objectType)
		{
			if (objectType == ObjectType.Device)
				return new List<Property> { Property.Description, Property.ShleifNo, Property.IntAddress, Property.DeviceState, Property.Type };
			if (objectType == ObjectType.Zone)
				return new List<Property> { Property.Description, Property.No, Property.Type };
			if (objectType == ObjectType.Direction)
				return new List<Property> { Property.Description, Property.No, Property.Delay, Property.Hold, Property.DelayRegime };
			return new List<Property>();
		}

		public static List<string> ObjectTypeToTypesList(ObjectType objectType)
		{
			if (objectType == ObjectType.Device)
				return XManager.Drivers.Select(x => x.Name).ToList();
			return new List<string>();
		}

		public static List<ConditionType> ObjectTypeToConditionTypesList(ValueType valueType)
		{
			if ((valueType == ValueType.Integer) || (valueType == ValueType.DateTime) || (valueType == ValueType.Object))
				return new List<ConditionType> { ConditionType.IsEqual, ConditionType.IsNotEqual, ConditionType.IsMore, ConditionType.IsNotMore, ConditionType.IsLess, ConditionType.IsNotLess};
			if (valueType == ValueType.Boolean)
				return new List<ConditionType> { ConditionType.IsEqual, ConditionType.IsNotEqual };
			if (valueType == ValueType.String)
				return new List<ConditionType> { ConditionType.StartsWith, ConditionType.EndsWith, ConditionType.Contains};
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
	}
}
