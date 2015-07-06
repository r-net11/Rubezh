using NLog;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace Common
{
	public static class Logger
	{
		private static NLog.Logger _logger;
		private static object[] _empty;

		static Logger()
		{
			Directory.SetCurrentDirectory(AppDomain.CurrentDomain.BaseDirectory);
			_logger = LogManager.GetCurrentClassLogger();
			_empty = new object[0];
		}

		public static void Error(Exception ex)
		{
			Error(ex, ex.Message, _empty);
		}

		public static void Error(Exception ex, string message)
		{
			Error(ex, message, _empty);
		}

		public static void Error(Exception ex, string message, params object[] args)
		{
			System.Diagnostics.Trace.WriteLine(message);
			lock (_logger)
				try
				{
					_logger.LogException(LogLevel.Error, string.Format(message, args), ex);
				}
				catch { }
		}

		public static void Error(string message)
		{
			Error(message, _empty);
		}

		public static void Error(string message, params object[] args)
		{
			System.Diagnostics.Trace.WriteLine(message);
			WriteLog(LogLevel.Error, message, args);
		}

		public static void Warn(string message)
		{
			Warn(message, _empty);
		}

		public static void Warn(string message, params object[] args)
		{
			WriteLog(LogLevel.Warn, message, args);
		}

		public static void Info(string message)
		{
			Info(message, _empty);
		}

		public static void Info(string message, params object[] args)
		{
			WriteLog(LogLevel.Info, message, args);
		}

		public static void Trace(string message)
		{
			Trace(message, _empty);
		}

		public static void Trace(string message, params object[] args)
		{
			WriteLog(LogLevel.Trace, message, args);
		}

		private static void WriteLog(LogLevel level, string message, params object[] args)
		{
			lock (_logger)
				try
				{
					_logger.Log(level, message, args);
				}
				catch
				{
				}
		}

		private static string GetStackTrace()
		{
			var stringBuilder = new StringBuilder();
			var stackTrace = new StackTrace(true);
			for (int i = 0; i < stackTrace.FrameCount; i++)
			{
				var stackFrame = stackTrace.GetFrame(i);
				var frameString = stackFrame.GetMethod().Name + " " + stackFrame.GetFileName() + ":" + stackFrame.GetFileLineNumber();
				stringBuilder.AppendLine(frameString);
			}
			return stringBuilder.ToString();
		}
	}
}