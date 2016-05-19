using System;
using System.Collections.Generic;

namespace RubezhService
{
	static class LogPresenter
	{
		public static List<LogItem> LogItems { get; private set; }
		static LogPresenter()
		{
			LogItems = new List<LogItem>();
		}
		public static void AddLog(string message, bool isError = false)
		{
			LogItems.Add(new LogItem(message, isError));
			PageController.OnPageChanged(Page.Log);
		}
	}

	class LogItem
	{
		public string Message { get; private set; }
		public bool IsError { get; private set; }
		public DateTime DateTime { get; private set; }

		public LogItem(string message, bool isError = false)
		{
			Message = message;
			IsError = isError;
			DateTime = DateTime.Now;
		}
	}
}
