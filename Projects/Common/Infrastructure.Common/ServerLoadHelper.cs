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
			string user = Environment.UserDomainName + "\\" + Environment.UserName;
			var registrySecurity = new RegistrySecurity();
			registrySecurity.AddAccessRule(new RegistryAccessRule(user,
				RegistryRights.WriteKey | RegistryRights.ChangePermissions,
				InheritanceFlags.None, PropagationFlags.None, AccessControlType.Deny));

			RegistryKey registryKey = Registry.LocalMachine.CreateSubKey("software\\rubezh\\Firesec-2", RegistryKeyPermissionCheck.Default, registrySecurity);
			if (registryKey != null)
			{
				registryKey.SetValue("FiresecServerPath", path);
			}
		}

		public static string GetLocation()
		{
			RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("software\\rubezh\\Firesec-2");
			if (registryKey != null)
			{
				var value = registryKey.GetValue("FiresecServerPath");
				if (value != null)
				{
					if (File.Exists((string)value))
						return (string)value;
				}
			}
			return null;
		}

		public static void SetStatus(FSServerState fsServerState)
		{
			string user = Environment.UserDomainName + "\\" + Environment.UserName;
			var registrySecurity = new RegistrySecurity();
			registrySecurity.AddAccessRule(new RegistryAccessRule(user,
				RegistryRights.WriteKey | RegistryRights.ChangePermissions,
				InheritanceFlags.None, PropagationFlags.None, AccessControlType.Deny));

			RegistryKey registryKey = Registry.LocalMachine.CreateSubKey("software\\rubezh\\Firesec-2", RegistryKeyPermissionCheck.Default, registrySecurity);
			if (registryKey != null)
			{
				registryKey.SetValue("FiresecServiceState", (int)fsServerState);
			}
		}

		public static FSServerState GetStatus()
		{
			RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("software\\rubezh\\Firesec-2");
			if (registryKey != null)
			{
				var value = registryKey.GetValue("FiresecServiceState");
				if (value != null)
				{
					return (FSServerState)value;
				}
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
	}

	public enum FSServerState
	{
		Closed = 0,
		Opening = 1,
		Opened = 2
	}
}