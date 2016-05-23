using System;
using System.Collections.Generic;

namespace RubezhService.Models
{
	static class LogModel
	{
		public static List<LogItem> LogItems { get; private set; }
		static LogModel()
		{
			LogItems = new List<LogItem>();
		}
		public static void AddLog(string message, bool isError = false)
		{
			LogItems.Add(new LogItem(message, isError));
            // TODO: Notify
        }
    }

	public class LogItem
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
