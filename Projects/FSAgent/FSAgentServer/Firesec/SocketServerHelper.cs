using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Common;
using Microsoft.Win32;
using Infrastructure.Common;

namespace FSAgentServer
{
	public class SocketServerHelper
	{
		public static void StartIfNotRunning()
		{
			foreach (var process in Process.GetProcesses())
			{
				if (process.ProcessName == "scktsrvr")
					return;
			}

			var fileName = GetSocketServerPath();
			if (fileName == null)
			{
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

		static void Stop()
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
				var directoryPath = GetPathFromRegistry();
				if (directoryPath != null)
				{
					var filePath1 = Path.Combine(directoryPath, "scktsrvr.exe");
					if (File.Exists(filePath1))
					{
						return filePath1;
					}
					else
					{
						Logger.Error("SocketServerHelper.GetSocketServerPath File1 Not Exists " + filePath1);
					}
				}
				var filePath2 = @"C:\Program Files\Firesec\scktsrvr.exe";
				if (File.Exists(filePath2))
				{
					return filePath2;
				}
				else
				{
					Logger.Error("SocketServerHelper.GetSocketServerPath File2 Not Exists " + filePath2);
				}
				var filePath3 = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), @"Firesec\scktsrvr.exe");
				if (File.Exists(filePath2))
				{
					return filePath3;
				}
				else
				{
					Logger.Error("SocketServerHelper.GetSocketServerPath File3 Not Exists " + filePath3);
				}

                LoadingErrorManager.Add("Не удалось обнаружить драйвер устройств");
				return null;

				//RegistryKey registryKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32).OpenSubKey(@"SYSTEM\CurrentControlSet\Services\Adv_SocketServer", false);
				//object key = registryKey.GetValue("ImagePath");
				//string path = key.ToString();
				//path = path.Replace(" -service", "");
				//path = path.Replace("\"", "");
				//return path;
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