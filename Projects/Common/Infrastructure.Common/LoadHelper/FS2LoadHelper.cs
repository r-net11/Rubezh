using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Common;

namespace Infrastructure.Common
{
	public class FS2LoadHelper
	{
		public static void SetLocation(string path)
		{
			RegistrySettingsHelper.SetString("FS2ServerPath", path);
		}

		public static string GetLocation()
		{
			var value = RegistrySettingsHelper.GetString("FS2ServerPath");
			if (value != null)
			{
				if (File.Exists((string)value))
					return (string)value;
			}
			return null;
		}

		public static void SetStatus(FS2State fs2State)
		{
			RegistrySettingsHelper.SetInt("FS2ServerState", (int)fs2State);
		}

		public static FS2State GetStatus()
		{
			var value = RegistrySettingsHelper.GetInt("FS2ServerState");
			try
			{
				return (FS2State)value;
			}
			catch { }
			return FS2State.Closed;
		}

		public static bool WaitUntinlStarted()
		{
			for (int i = 0; i < 100; i++)
			{
				var fs2State = GetStatus();
				if (fs2State == FS2State.Opened)
					return true;
				Thread.Sleep(TimeSpan.FromSeconds(1));
			}
			return false;
		}

		public static bool Load()
		{
			Process[] processes = Process.GetProcessesByName("ServerFS2");
			Process[] processesVsHost = Process.GetProcessesByName("ServerFS2.vshost");
			Process[] processesVsHost2 = Process.GetProcessesByName("MonitorTestClientFS2.vshost");
			var isRunning = false;
			foreach (var process in processes)
			{
				var isCurrentUser = ProcessHelper.IsCurrentUserProcess(process.Id);
				if (isCurrentUser)
					isRunning = true;
			}
			if (processes.Count() + processesVsHost.Count() + processesVsHost2.Count() == 0)
			{
				try
				{
					SetStatus(FS2State.Closed);
					if (!Start())
						return false;
					if (!WaitUntinlStarted())
						return false;
				}
				catch (Exception e)
				{
					Logger.Error(e, "FS2LoadHelper.Load");
					return false;
				}
			}
			return true;
		}

		static bool Start()
		{
			var fileName = @"..\FS2\ServerFS2.exe";
			if (!File.Exists(fileName))
			{
				fileName = GetLocation();
				if (fileName == null)
					return false;
			}
			if (!File.Exists(fileName))
			{
				Logger.Error("FS2LoadHelper.Start File Not Exist " + fileName);
				return false;
			}
			System.Diagnostics.Process process = new System.Diagnostics.Process();
			process.StartInfo.FileName = fileName;
			process.Start();
			return true;
		}
	}

	public enum FS2State
	{
		Closed = 0,
		Opening = 1,
		Opened = 2
	}
}