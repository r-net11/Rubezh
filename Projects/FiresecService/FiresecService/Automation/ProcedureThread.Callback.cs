using StrazhAPI.Automation;
using StrazhAPI.AutomationCallback;
using System;
using System.Collections.Concurrent;
using System.Threading;

namespace FiresecService
{
	public partial class ProcedureThread
	{
		private static ConcurrentDictionary<Guid, ProcedureThread> _proceduresThreads = new ConcurrentDictionary<Guid, ProcedureThread>();
		private AutoResetEvent _waitHandler;
		private object _callbackResponse;

		private object SendCallback(UIArguments arguments, AutomationCallbackResult callback, bool withResponse = false)
		{
			callback.CallbackUID = Guid.NewGuid();
			if (callback.Data != null)
				callback.Data.LayoutFilter = GetLayoutFilter(arguments);
			_callbackResponse = null;
			if (withResponse)
			{
				using (_waitHandler = new AutoResetEvent(false))
				{
					_proceduresThreads.GetOrAdd(callback.CallbackUID, this);
					Service.FiresecService.NotifyAutomation(callback, GetClientUID(arguments));
					if (!_waitHandler.WaitOne(TimeSpan.FromMinutes(1)))
						CallbackResponse(callback.CallbackUID, null);
				}
			}
			else
				Service.FiresecService.NotifyAutomation(callback, GetClientUID(arguments));
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
			return arguments.ForAllClients ? null : ClientUID;
		}

		private ProcedureLayoutCollection GetLayoutFilter(UIArguments arguments)
		{
			return arguments.LayoutFilter == null || arguments.LayoutFilter.LayoutsUIDs == null || arguments.LayoutFilter.LayoutsUIDs.Count == 0 ? null : arguments.LayoutFilter;
		}
	}
}