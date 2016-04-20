using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Common;
using Microsoft.Win32;
using Infrastructure.Common.Windows;
using System.Collections.Generic;

namespace FSAgentServer
{
	public class SocketServerHelper
	{
		public static void StartIfNotRunning()
		{
			foreach (var process in Process.GetProcesses())
			{
				if (process.ProcessName == "scktsrvr" || process.ProcessName == "ScktSrvr")
					return;
			}

			var fileName = GetSocketServerPath();
			if (fileName == null)
			{
				Logger.Error("NativeFiresecClient.StartSocketServerIfNotRunning Не найден файл " + fileName);
				return;
			}

			var newProcess = new Process();
			newProcess.StartInfo = new ProcessStartInfo()
			{
				FileName = fileName
			};
			newProcess.Start();
			newProcess.WaitForInputIdle(1000);

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
			foreach (var process in Process.GetProcesses())
			{
				if (process.ProcessName == "FS_SER~1")
				{
					process.Kill();
					process.WaitForExit(1000);
				}
			}

			foreach (var process in Process.GetProcesses())
			{
				if (process.ProcessName == "fs_server")
				{
					process.Kill();
					process.WaitForExit(1000);
				}
			}

			foreach (var process in Process.GetProcesses())
			{
				if (process.ProcessName == "scktsrvr")
				{
					process.Kill();
					process.WaitForExit(1000);
				}
			}
		}

		public static void Restart()
		{
			Logger.Error("SocketServerHelper.Restart");
			Stop();
			StartIfNotRunning();
		}

		public static void RestartIfNotRunning()
		{
			Logger.Error("SocketServerHelper.RestartIfNotRunning");
			StartIfNotRunning();
		}

		static string GetSocketServerPath()
		{
			try
			{
				var names = new List<string>();
				names.Add(@"..\Firesec5\scktsrvr.exe");
				names.Add(@"..\..\Firesec5\scktsrvr.exe");
				names.Add(@"..\..\Firesec\scktsrvr.exe");
				names.Add(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"Rubezh\Firesec5\scktsrvr.exe"));
				names.Add(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"Firesec\scktsrvr.exe"));
				names.Add(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"Firesec5\scktsrvr.exe"));
				names.Add(@"C:\Program Files\Rubezh\Firesec5\scktsrvr.exe");
				names.Add(@"C:\Program Files\Firesec5\scktsrvr.exe");

				foreach (var name in names)
				{
					if (File.Exists(name))
					{
						return name;
					}
				}

				var directoryPath = GetPathFromRegistry();
				if (directoryPath != null)
				{
					var filePath = Path.Combine(directoryPath, "scktsrvr.exe");
					if (File.Exists(filePath))
					{
						return filePath;
					}
				}

				LoadingErrorManager.Add("Не удалось обнаружить драйвер устройств");
				return null;

				//var fileName00 = @"..\..\Firesec5\scktsrvr.exe";
				//if (File.Exists(fileName00))
				//{
				//	return fileName00;
				//}
				//else
				//{
				//	Logger.Error("SocketServerHelper.GetSocketServerPath File00 Not Exists " + fileName00);
				//}

				//var fileName01 = @"..\..\Firesec\scktsrvr.exe";
				//if (File.Exists(fileName01))
				//{
				//	return fileName01;
				//}
				//else
				//{
				//	Logger.Error("SocketServerHelper.GetSocketServerPath File01 Not Exists " + fileName01);
				//}

				//var directoryPath = GetPathFromRegistry();
				//if (directoryPath != null)
				//{
				//	var filePath1 = Path.Combine(directoryPath, "scktsrvr.exe");
				//	if (File.Exists(filePath1))
				//	{
				//		return filePath1;
				//	}
				//	else
				//	{
				//		Logger.Error("SocketServerHelper.GetSocketServerPath File1 Not Exists " + filePath1);
				//	}
				//}
				//var filePath2 = @"C:\Program Files\Firesec\scktsrvr.exe";
				//if (File.Exists(filePath2))
				//{
				//	return filePath2;
				//}
				//else
				//{
				//	Logger.Error("SocketServerHelper.GetSocketServerPath File2 Not Exists " + filePath2);
				//}
				//var filePath3 = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"Firesec\scktsrvr.exe");
				//if (File.Exists(filePath3))
				//{
				//	return filePath3;
				//}
				//else
				//{
				//	Logger.Error("SocketServerHelper.GetSocketServerPath File3 Not Exists " + filePath3);
				//}
				//var filePath4 = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), @"Firesec5\scktsrvr.exe");
				//if (File.Exists(filePath4))
				//{
				//	return filePath4;
				//}
				//else
				//{
				//	Logger.Error("SocketServerHelper.GetSocketServerPath File4 Not Exists " + filePath4);
				//}
				//var filePath5 = @"C:\Program Files\Firesec5\scktsrvr.exe";
				//if (File.Exists(filePath5))
				//{
				//	return filePath5;
				//}
				//else
				//{
				//	Logger.Error("SocketServerHelper.GetSocketServerPath File5 Not Exists " + filePath5);
				//}

				//LoadingErrorManager.Add("Не удалось обнаружить драйвер устройств");
				//return null;

				////RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(@"SYSTEM\CurrentControlSet\Services\Adv_SocketServer", false);
				////object key = registryKey.GetValue("ImagePath");
				////string path = key.ToString();
				////path = path.Replace(" -service", "");
				////path = path.Replace("\"", "");
				////return path;
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове NativeFiresecClient.GetSocketServerPath");
				LoadingErrorManager.Add(e);
				return null;
			}
		}

		static string GetPathFromRegistry()
		{
			string directoryPath = null;
			RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.ClassesRoot, RegistryView.Registry64).OpenSubKey(@"TypeLib\{B86C6AD7-5766-4233-AF55-707B45661224}\1.0\HELPDIR", false);
			if (registryKey != null)
			{
				object key = registryKey.GetValue("");
				if (key != null)
				{
					directoryPath = key.ToString();
				}
				else
				{
					Logger.Error("SocketServerHelper.GetPathFromRegistry key = null");
				}
			}
			else
			{
				Logger.Error("SocketServerHelper.GetPathFromRegistry registryKey = null");
			}
			return directoryPath;
		}
	}
}