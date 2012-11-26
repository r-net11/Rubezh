using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Common;
using System.IO;
using System.Threading;
using Microsoft.Win32;

namespace Infrastructure.Common
{
	public class FSAgentLoadHelper
	{
		public static void NotifyStarting()
		{
			RegistryKey registryKey = Registry.LocalMachine.CreateSubKey("software\\rubezh\\Firesec-2");
			registryKey.SetValue("FSAgentServerPath", System.Reflection.Assembly.GetExecutingAssembly().Location);
			registryKey.SetValue("FSAgentServerState", "Starting");
		}

		public static void NotifyStartCompleted()
		{
			RegistryKey registryKey = Registry.LocalMachine.CreateSubKey("software\\rubezh\\Firesec-2");
			registryKey.SetValue("FSAgentServerState", "Ready");
		}

		public static void Load()
		{
			Process[] processes = Process.GetProcessesByName("FSAgentServer");
			if (processes.Count() == 0)
			{
				try
				{
					Start();
				}
				catch (Exception e)
				{
					Logger.Error(e, "FSAgentLoadHelper.Load");
				}
			}
		}

		public static void Reload()
		{
			Start();
			for (int i = 0; i < 100; i++)
			{
				RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("software\\rubezh\\Firesec-2");
				if (registryKey != null)
				{
					var value = registryKey.GetValue("FSAgentServerState");
					if (value != null)
					{
						if (value == "Ready")
							return;
					}
				}
				Thread.Sleep(TimeSpan.FromSeconds(1));
			}
		}

		static void Start()
		{
#if DEBUG
			return;
#endif

			System.Diagnostics.Process proc = new System.Diagnostics.Process();
			var fileName = @"..\FSAgent\FSAgentServer.exe";

			if (!File.Exists(fileName))
			{
				Logger.Error("ServerLoadHelper.Start File Not Exist " + fileName);
			}
			proc.StartInfo.FileName = fileName;
			proc.Start();
		}
	}
}