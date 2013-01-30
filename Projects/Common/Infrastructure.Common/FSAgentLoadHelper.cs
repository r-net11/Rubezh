using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Common;
using Microsoft.Win32;

namespace Infrastructure.Common
{
	public class FSAgentLoadHelper
	{
		public static void SetLocation(string path)
		{
			RegistrySettingsHelper.Set("FSAgentServerPath", path);
		}

		public static string GetLocation()
		{
			var value = RegistrySettingsHelper.Get("FSAgentServerPath");
			if (value != null)
			{
				if (File.Exists((string)value))
					return (string)value;
			}
			return null;
		}

		public static void SetStatus(FSAgentState fsAgentState)
		{
			RegistrySettingsHelper.Set("FSAgentServerState", fsAgentState.ToString());
		}

		public static FSAgentState GetStatus()
		{
			var value = RegistrySettingsHelper.Get("FSAgentServerState");
			if (value != null)
			{
				try
				{
					return (FSAgentState)Enum.Parse(typeof(FSAgentState), value);
				}
				catch { }
			}
			return FSAgentState.Closed;
		}

		public static bool WaitUntinlStarted()
		{
			for (int i = 0; i < 100; i++)
			{
				var fsAgentState = GetStatus();
				if (fsAgentState == FSAgentState.Opened)
					return true;
				Thread.Sleep(TimeSpan.FromSeconds(1));
			}
			return false;
		}

		public static bool Load()
		{
			Process[] processes = Process.GetProcessesByName("FSAgentServer");
			Process[] processesVsHost = Process.GetProcessesByName("FSAgentServer.vshost");
			if ((processes.Count() == 0) && (processesVsHost.Count() == 0))
			{
				try
				{
					SetStatus(FSAgentState.Closed);
					if (!Start())
						return false;
					if (!WaitUntinlStarted())
						return false;
				}
				catch (Exception e)
				{
					Logger.Error(e, "FSAgentLoadHelper.Load");
					return false;
				}
			}
			return true;
		}

		static bool Start()
		{
			var fileName = @"..\FSAgent\FSAgentServer.exe";
			if (!File.Exists(fileName))
			{
				fileName = GetLocation();
				if (fileName == null)
					return false;
			}
			if (!File.Exists(fileName))
			{
				Logger.Error("FSAgentLoadHelper.Start File Not Exist " + fileName);
				return false;
			}
			System.Diagnostics.Process process = new System.Diagnostics.Process();
			process.StartInfo.FileName = fileName;
			process.Start();
			return true;
		}
	}

	public enum FSAgentState
	{
		Closed = 0,
		Opening = 1,
		Opened = 2
	}
}