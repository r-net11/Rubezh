using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Infrastructure.Common
{
	public static class StackTraceHelper
	{
		public static string GetStackTrace()
		{
			var stackTrace = new StackTrace(true);
			var stackFrame = stackTrace.GetFrame(1);
			return stackFrame.GetMethod().Name + " " + stackFrame.GetFileName() + ":" + stackFrame.GetFileLineNumber();
		}
	}
}