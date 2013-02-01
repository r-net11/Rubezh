using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Common;
using Microsoft.Win32;
using System.Security.AccessControl;

namespace Infrastructure.Common
{
	public static class ServerLoadHelper
	{
		public static void SetLocation(string path)
		{
			var stackTrace = GetStackTrace();
			RegistrySettingsHelper.SetString("FiresecServerPath", path);
		}

		public static string GetLocation()
		{
			var value = RegistrySettingsHelper.GetString("FiresecServerPath");
			if (value != null)
			{
				if (File.Exists((string)value))
					return (string)value;
			}
			return null;
		}

		public static void SetStatus(FSServerState fsServerState)
		{
			RegistrySettingsHelper.SetInt("FiresecServiceState", (int)fsServerState);
		}

		public static FSServerState GetStatus()
		{
			var value = RegistrySettingsHelper.GetInt("FiresecServiceState");
			try
			{
				return (FSServerState)value;
			}
			catch { }
			return FSServerState.Closed;
		}

		public static bool WaitUntinlStarted()
		{
			for (int i = 0; i < 100; i++)
			{
				var fsAgentState = GetStatus();
				if (fsAgentState == FSServerState.Opened)
					return true;
				Thread.Sleep(TimeSpan.FromSeconds(1));
			}
			return false;
		}

		public static bool Load()
		{
			Process[] processes = Process.GetProcessesByName("FiresecService");
			Process[] processesVsHost = Process.GetProcessesByName("FiresecService.vshost");
			if ((processes.Count() == 0) && (processesVsHost.Count() == 0))
			{
				try
				{
					SetStatus(FSServerState.Closed);
					if (!Start())
						return false;
					if (!WaitUntinlStarted())
						return false;
				}
				catch (Exception e)
				{
					Logger.Error(e, "ServerLoadHelper.Load");
					return false;
				}
			}
			return true;
		}

		static bool Start()
		{
			var fileName = @"..\FiresecService\FiresecService.exe";
			if (!File.Exists(fileName))
			{
				fileName = GetLocation();
				if (fileName == null)
					return false;
			}
			if (!File.Exists(fileName))
			{
				Logger.Error("ServerLoadHelper.Start File Not Exist " + fileName);
				return false;
			}
			System.Diagnostics.Process process = new System.Diagnostics.Process();
			process.StartInfo.FileName = fileName;
			process.Start();
			return true;
		}

		static string GetStackTrace()
		{
			var stackTrace = new StackTrace(true);
			var stackFrame = stackTrace.GetFrame(1);
			return stackFrame.GetMethod().Name + " " + stackFrame.GetFileName() + ":" + stackFrame.GetFileLineNumber();
		}
	}

	public enum FSServerState
	{
		Closed = 0,
		Opening = 1,
		Opened = 2
	}
}