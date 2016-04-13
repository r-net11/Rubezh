using Common;
using RubezhAPI.Automation;
using RubezhAPI.AutomationCallback;
using RubezhAPI.Models;
using System;
using System.Collections.Generic;

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

		public static void InitializeGlobalVariables()
		{
			GlobalVariables = SystemConfiguration != null ?
				Utils.Clone(SystemConfiguration.AutomationConfiguration.GlobalVariables) :
				new List<Variable>();
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

		public static void SynchronizeVariable(Variable variable, Guid? initialClientUID)
		{
			if (initialClientUID.HasValue && variable.IsGlobal && variable.ContextType == ContextType.Server && OnSynchronizeVariable != null)
				OnSynchronizeVariable(initialClientUID.Value, variable);
		}

		public static void SetVariableValue(Variable target, object value, Guid? initialClientUID)
		{
			if (target == null)
				return;

			target.Value = value;

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
						ExplicitValue = (ExplicitValue)target
					}
				}, null);
			else
				SynchronizeVariable(target, initialClientUID);
			//target.OnValueChanged();
		}
	}
}
