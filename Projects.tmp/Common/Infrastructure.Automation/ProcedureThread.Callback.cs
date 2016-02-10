﻿using RubezhAPI.Automation;
using RubezhAPI.AutomationCallback;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace Infrastructure.Automation
{
	public partial class ProcedureThread
	{
		private static ConcurrentDictionary<Guid, ProcedureThread> _proceduresThreads = new ConcurrentDictionary<Guid, ProcedureThread>();
		private AutoResetEvent _waitHandler;
		private object _callbackResponse;

		private object SendCallback(UIArguments arguments, AutomationCallbackResult callback, bool withResponse = false)
		{
			callback.CallbackUID = Guid.NewGuid();
			callback.ContextType = this.ContextType;
			if (callback.Data is UIAutomationCallbackData)
				(callback.Data as UIAutomationCallbackData).LayoutFilter = GetLayoutFilter(arguments);
			_callbackResponse = null;
			if (withResponse)
			{
				using (_waitHandler = new AutoResetEvent(false))
				{
					_proceduresThreads.GetOrAdd(callback.CallbackUID, this);
					ProcedureExecutionContext.SendCallback(callback, GetClientUID(arguments));
					if (!_waitHandler.WaitOne(TimeSpan.FromMinutes(1)))
						CallbackResponse(callback.CallbackUID, null);
				}
			}
			else
				ProcedureExecutionContext.SendCallback(callback, GetClientUID(arguments));
			return _callbackResponse;
		}

		public static void CallbackResponse(Guid callbackUID, object value)
		{
			ProcedureThread procedureThread;
			if (_proceduresThreads.TryRemove(callbackUID, out procedureThread))
				procedureThread.CallbackResponse(value);
		}
		private void CallbackResponse(object value)
		{
			_callbackResponse = value;
			_waitHandler.Set();
		}

		private Guid? GetClientUID(UIArguments arguments)
		{
			return arguments == null || arguments.ForAllClients ? null : (Guid?)ClientUID;
		}
		private List<Guid> GetLayoutFilter(UIArguments arguments)
		{
			return arguments == null || arguments.LayoutFilter == null || arguments.LayoutFilter.Count == 0 ? null : arguments.LayoutFilter;
		}
	}
}