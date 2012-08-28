using System;
using System.Linq;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using Common;
using Microsoft.Win32;

namespace Firesec
{
	public class SocketServerHelper
	{
		[Obsolete]
		public static void StartNTServiceIfNotRunning()
		{
			var service = new System.ServiceProcess.ServiceController("Borland Advanced Socket Server");
			if ((service != null) && (service.Status != System.ServiceProcess.ServiceControllerStatus.Running))
			{
				try
				{
					service.Start();
					service.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromMilliseconds(10000));
				}
				catch (Exception e)
				{
					Logger.Error(e, "Исключение при вызове NativeFiresecClient.StartNTServiceIfNotRunning");
				}
			}
		}

		public static void StopNTServiceIfRunning()
		{
			var service = (from srv in System.ServiceProcess.ServiceController.GetServices()
						   where srv.ServiceName == "Borland Advanced Socket Server"
						   select srv).FirstOrDefault();
			if ((service != null) && (service.Status == System.ServiceProcess.ServiceControllerStatus.Running))
			{
				try
				{
					service.Stop();
					service.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromMilliseconds(10000));
				}
				catch (Exception e)
				{
					Logger.Error(e, "Исключение при вызове NativeFiresecClient.StopNTServiceIfNotRunning");
				}
			}
		}

		public static void StartIfNotRunning()
		{
			foreach (var process in Process.GetProcesses())
			{
				if (process.ProcessName == "scktsrvr")
					return;
			}

			var newProcess = new Process();
			newProcess.StartInfo = new ProcessStartInfo()
			{
				FileName = GetSocketServerPath()
			};
			newProcess.Start();

			for (int i = 0; i < 100; i++)
			{
				foreach (var process in Process.GetProcesses())
				{
					if (process.ProcessName == "scktsrvr")
					{
						return;
					}
				}
				Thread.Sleep(100);
			}
			Logger.Error("NativeFiresecClient.StartSocketServerIfNotRunning не удалось запустить процесс scktsrvr");
		}

		public static void Stop()
		{
			StopNTServiceIfRunning();

			foreach (var process in Process.GetProcesses())
			{
				if (process.ProcessName == "FS_SER~1")
					process.Kill();
			}

			foreach (var process in Process.GetProcesses())
			{
				if (process.ProcessName == "scktsrvr")
					process.Kill();
			}
		}

		static string GetSocketServerPath()
		{
			try
			{
				RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64).OpenSubKey(@"SYSTEM\CurrentControlSet\Services\Adv_SocketServer", false);
				object key = registryKey.GetValue("ImagePath");
				string path = key.ToString();
				path = path.Replace(" -service", "");
				path = path.Replace("\"", "");
				return path;
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове NativeFiresecClient.GetSocketServerPath");
				return System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"Firesec\scktsrvr.exe");
			}
		}
	}
}