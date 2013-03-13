using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Common;

namespace Infrastructure.Common
{
	public static class ServerLoadHelper
	{
		public static void SetLocation(string path)
		{
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
			RegistrySettingsHelper.SetInt("FiresecService.State", (int)fsServerState);
			Logger.Info("ServerLoadHelper.SetStatus FiresecService.State = " + fsServerState.ToString());
		}

		public static FSServerState GetStatus()
		{
			var value = RegistrySettingsHelper.GetInt("FiresecService.State");
			try
			{
				Logger.Info("ServerLoadHelper.GetStatus FiresecService.State = " + ((FSServerState)value).ToString());
				return (FSServerState)value;
			}
			catch(Exception e)
			{
				Logger.Error(e, "ServerLoadHelper.GetStatus");
			}
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
			var isRunning = false;
			foreach (var process in processes)
			{
				var isCurrentUser = ProcessHelper.IsCurrentUserProcess(process.Id);
				if (isCurrentUser)
					isRunning = true;
			}
			if (!isRunning && (processesVsHost.Count() == 0))
			//if ((processes.Count() == 0) && (processesVsHost.Count() == 0))
			{
				try
				{
					Logger.Info("isRunning: " + isRunning.ToString());
					foreach (var process in processes)
					{
						Logger.Info("Processes: " + process.ProcessName);
					}
					foreach (var process in processes)
					{
						Logger.Info("processesVsHost: " + process.ProcessName);
					}
					SetStatus(FSServerState.Closed);
					if (!Start())
					{
						Logger.Error("ServerLoadHelper.Load !Start");
						return false;
					}
					if (!WaitUntinlStarted())
					{
						processes = Process.GetProcessesByName("FiresecService");
						Logger.Info("Processes.Count=" + processes.Count().ToString());

						Logger.Error("ServerLoadHelper.Load !WaitUntinlStarted");
						return false;
					}
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
				{
					Logger.Error("ServerLoadHelper.Start File Not Exist " + fileName);
					return false;
				}
			}
			if (!File.Exists(fileName))
			{
				Logger.Error("ServerLoadHelper.Start File Not Exist " + fileName);
				return false;
			}
			System.Diagnostics.Process process = new System.Diagnostics.Process();
			process.StartInfo.FileName = fileName;
			process.Start();
			Logger.Info("Server process is starting up " + fileName);
			return true;
		}
	}

	public enum FSServerState
	{
		Closed = 0,
		Opening = 1,
		Opened = 2
	}
}