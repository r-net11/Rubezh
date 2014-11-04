using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FiresecAPI.Automation;
using FiresecAPI.AutomationCallback;
using System.Threading;

namespace FiresecService
{
	public partial class ProcedureThread
	{
		private object _lock = new object();
		private object _callbackResponse;
		private AutoResetEvent _waitHandler;

		private object SendCallback(UIArguments arguments, AutomationCallbackResult callback, bool withResponse = false)
		{
			callback.ProcedureUID = UID;
			if (callback.Data != null)
				callback.Data.LayoutFilter = GetLayoutFilter(arguments);
			Service.FiresecService.NotifyAutomation(callback, GetClientUID(arguments));
			_callbackResponse = null;
			if (withResponse)
			{
				using (_waitHandler = new AutoResetEvent(false))
					_waitHandler.WaitOne(TimeSpan.FromMinutes(1));
				_waitHandler = null;
			}
			return _callbackResponse;
		}

		public void SetCallbackResponse(object value)
		{
			if (_waitHandler != null)
				lock (_lock)
					if (_waitHandler != null)
					{
						_callbackResponse = value;
						_waitHandler.Set();
						while (_waitHandler != null)
							Thread.Sleep(100);
					}
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
