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

		public static void SetVariableValue(Variable target, object value, Guid? initialClientUID)
		{
			if (target == null)
				return;

			if (value is IEnumerable<object>)
			{
				var listValue = value as IEnumerable<object>;

				if (target.ExplicitType == ExplicitType.Integer)
					target.ExplicitValues = listValue.Select(x => new ExplicitValue() { IntValue = Convert.ToInt32(x), ExplicitType = target.ExplicitType }).ToList();
				if (target.ExplicitType == ExplicitType.Float)
					target.ExplicitValues = listValue.Select(x => new ExplicitValue() { FloatValue = Convert.ToDouble(x), ExplicitType = target.ExplicitType }).ToList();
				if (target.ExplicitType == ExplicitType.String)
					target.ExplicitValues = listValue.Select(x => new ExplicitValue() { StringValue = Convert.ToString(x), ExplicitType = target.ExplicitType }).ToList();
				if (target.ExplicitType == ExplicitType.Boolean)
					target.ExplicitValues = listValue.Select(x => new ExplicitValue() { BoolValue = Convert.ToBoolean(x), ExplicitType = target.ExplicitType }).ToList();
				if (target.ExplicitType == ExplicitType.DateTime)
					target.ExplicitValues = listValue.Select(x => new ExplicitValue() { DateTimeValue = Convert.ToDateTime(x), ExplicitType = target.ExplicitType }).ToList();
				if (target.ExplicitType == ExplicitType.Object)
					target.ExplicitValues = listValue.Select(x => new ExplicitValue() { ObjectReferenceValue = AutomationHelper.GetObjectReference(value), ExplicitType = target.ExplicitType }).ToList();
				if (target.ExplicitType == ExplicitType.Enum)
				{
					if (target.EnumType == EnumType.DriverType)
						target.ExplicitValues = listValue.Select(x => new ExplicitValue() { DriverTypeValue = (GKDriverType)x, ExplicitType = target.ExplicitType }).ToList();
					if (target.EnumType == EnumType.StateType)
						target.ExplicitValues = listValue.Select(x => new ExplicitValue() { StateTypeValue = (XStateClass)x, ExplicitType = target.ExplicitType }).ToList();
					if (target.EnumType == EnumType.PermissionType)
						target.ExplicitValues = listValue.Select(x => new ExplicitValue() { PermissionTypeValue = (PermissionType)x, ExplicitType = target.ExplicitType }).ToList();
					if (target.EnumType == EnumType.JournalEventNameType)
						target.ExplicitValues = listValue.Select(x => new ExplicitValue() { JournalEventNameTypeValue = (JournalEventNameType)x, ExplicitType = target.ExplicitType }).ToList();
					if (target.EnumType == EnumType.JournalEventDescriptionType)
						target.ExplicitValues = listValue.Select(x => new ExplicitValue() { JournalEventDescriptionTypeValue = (JournalEventDescriptionType)x, ExplicitType = target.ExplicitType }).ToList();
					if (target.EnumType == EnumType.JournalObjectType)
						target.ExplicitValues = listValue.Select(x => new ExplicitValue() { JournalObjectTypeValue = (JournalObjectType)x, ExplicitType = target.ExplicitType }).ToList();
					if (target.EnumType == EnumType.ColorType)
						target.ExplicitValues = listValue.Select(x => new ExplicitValue() { ColorValue = (Color)x, ExplicitType = target.ExplicitType }).ToList();
				}
			}
			else
			{
				if (target.ExplicitType == ExplicitType.Integer)
				{
					target.ExplicitValue.IntValue = Convert.ToInt32(value);
					target.ExplicitValue.ExplicitType = target.ExplicitType;
				}
				if (target.ExplicitType == ExplicitType.Float)
				{
					target.ExplicitValue.FloatValue = Convert.ToDouble(value);
					target.ExplicitValue.ExplicitType = target.ExplicitType;
				}
				if (target.ExplicitType == ExplicitType.String)
				{
					target.ExplicitValue.StringValue = Convert.ToString(value);
					target.ExplicitValue.ExplicitType = target.ExplicitType;
				}
				if (target.ExplicitType == ExplicitType.Boolean)
				{
					target.ExplicitValue.BoolValue = Convert.ToBoolean(value);
					target.ExplicitValue.ExplicitType = target.ExplicitType;
				}
				if (target.ExplicitType == ExplicitType.DateTime)
				{
					target.ExplicitValue.DateTimeValue = Convert.ToDateTime(value);
					target.ExplicitValue.ExplicitType = target.ExplicitType;
				}
				if (target.ExplicitType == ExplicitType.Object)
				{
					target.ExplicitValue.ObjectReferenceValue = AutomationHelper.GetObjectReference(value);
					target.ExplicitValue.ExplicitType = target.ExplicitType;
				}
				if (target.ExplicitType == ExplicitType.Enum)
				{
					if (target.EnumType == EnumType.DriverType)
					{
						target.ExplicitValue.DriverTypeValue = (GKDriverType)value;
						target.ExplicitValue.ExplicitType = target.ExplicitType;
					}
					if (target.EnumType == EnumType.StateType)
					{
						target.ExplicitValue.StateTypeValue = (XStateClass)value;
						target.ExplicitValue.ExplicitType = target.ExplicitType;
					}
					if (target.EnumType == EnumType.PermissionType)
					{
						target.ExplicitValue.PermissionTypeValue = (PermissionType)value;
						target.ExplicitValue.ExplicitType = target.ExplicitType;
					}
					if (target.EnumType == EnumType.JournalEventNameType)
					{
						target.ExplicitValue.JournalEventNameTypeValue = (JournalEventNameType)value;
						target.ExplicitValue.ExplicitType = target.ExplicitType;
					}
					if (target.EnumType == EnumType.JournalEventDescriptionType)
					{
						target.ExplicitValue.JournalEventDescriptionTypeValue = (JournalEventDescriptionType)value;
						target.ExplicitValue.ExplicitType = target.ExplicitType;
					}
					if (target.EnumType == EnumType.JournalObjectType)
					{
						target.ExplicitValue.JournalObjectTypeValue = (JournalObjectType)value;
						target.ExplicitValue.ExplicitType = target.ExplicitType;
					}
					if (target.EnumType == EnumType.ColorType)
					{
						target.ExplicitValue.ColorValue = (Color)value;
						target.ExplicitValue.ExplicitType = target.ExplicitType;
					}
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
