using RubezhAPI.Automation;
using RubezhAPI.AutomationCallback;
using RubezhAPI.GK;
using RubezhAPI.Journal;
using RubezhAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace Infrastructure.Automation
{
	public partial class ProcedureExecutionContext
	{
		internal static ContextType ContextType { get; private set; }
		public static SystemConfiguration SystemConfiguration
		{
			get
			{
				if (_getSystemConfiguration != null)
					return _getSystemConfiguration();
				else
					throw new InvalidOperationException("ProcedureExecutionContext не инициализирован");
			}
		}

		public static List<Variable> GlobalVariables { get; private set; }

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

		public static void SynchronizeVariable(Variable variable, Guid? initialClientUID)
		{
			if (initialClientUID.HasValue && variable.IsGlobal && variable.ContextType == ContextType.Server && OnSynchronizeVariable != null)
				OnSynchronizeVariable(initialClientUID.Value, variable);
		}

		public static object GetVariableValue(Guid clientUID, Variable source)
		{
			return GetValue(source);
		}

		public static object GetValue(Variable variable)
		{
			return variable == null ? null : variable.Value;
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
				case ExplicitType.Float:
					return explicitValue.FloatValue;
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
				case ExplicitType.Float:
					return explicitValues.Select(x => (object)x.FloatValue).ToArray();
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

		public static void SetVariableValue(Variable target, object value, Guid? initialClientUID)
		{
			if (target == null)
				return;

			if (value is IEnumerable<object>)
			{
				var listValue = value as IEnumerable<object>;

				if (target.ExplicitType == ExplicitType.Integer)
					target.ExplicitValues = listValue.Select(x => new ExplicitValue() { IntValue = Convert.ToInt32(x) }).ToList();
				if (target.ExplicitType == ExplicitType.Float)
					target.ExplicitValues = listValue.Select(x => new ExplicitValue() { FloatValue = Convert.ToDouble(x) }).ToList();
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
				if (target.ExplicitType == ExplicitType.Float)
					target.ExplicitValue.FloatValue = Convert.ToDouble(value);
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
			if (ContextType == ContextType.Server && target.IsGlobal && target.ContextType == ContextType.Server)
				SendCallback(new AutomationCallbackResult
				{
					CallbackUID = Guid.NewGuid(),
					ContextType = ContextType.Server,
					AutomationCallbackType = AutomationCallbackType.GlobalVariable,
					Data = new GlobalVariableCallBackData
					{
						InitialClientUID = initialClientUID,
						VariableUID = target.Uid,
						ExplicitValue = target.ExplicitValue,
						ExplicitValues = target.ExplicitValues
					}
				}, null);
			else
				SynchronizeVariable(target, initialClientUID);
			target.OnValueChanged();
		}
	}
}
