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
		private bool _flag;

		private object SendCallback(UIArguments arguments, AutomationCallbackResult callback, bool withResponse = false)
		{
			callback.ProcedureUID = UID;
			if (callback.Data != null)
				callback.Data.LayoutFilter = GetLayoutFilter(arguments);
			if (withResponse)
				_flag = true;
			Service.FiresecService.NotifyAutomation(callback, GetClientUID(arguments));
			_callbackResponse = null;
			if (withResponse)
			{
				using (_waitHandler = new AutoResetEvent(false))
					_waitHandler.WaitOne(TimeSpan.FromMinutes(1));
				_waitHandler = null;
			}
			_flag = false;
			return _callbackResponse;
		}

		public void SetCallbackResponse(object value)
		{
			if (_flag)
				lock (_lock)
					if (_flag)
					{
						_callbackResponse = value;
						_waitHandler.Set();
						_flag = false;
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
