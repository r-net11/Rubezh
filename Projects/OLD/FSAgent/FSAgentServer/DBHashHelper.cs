using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using Common;

namespace FSAgentServer
{
	public static class DBHashHelper
	{
		public static void RemoveHash()
		{
			try
			{
				var directoryNames = new List<string>();
				directoryNames.Add("C:/Program Files/Firesec5");
				directoryNames.Add("C:/Program Files/Firesec");
				directoryNames.Add("C:/Program Files(x86)/Firesec5");
				directoryNames.Add("C:/Program Files(x86)/Firesec");
				directoryNames.Add("C:/Firesec");
				var runningFSServerName = GetDirectoryName();
				if (runningFSServerName != null)
				{
					directoryNames.Add(runningFSServerName);
				}
				foreach (var directoryName in directoryNames)
				{
					var name = Path.Combine(directoryName, "Data", "DBHash");
					if (Directory.Exists(name))
					{
						var directoryInfo = new DirectoryInfo(name);
						foreach (var file in directoryInfo.GetFiles())
						{
							file.Delete();
						}
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "DBHashHelper.RemoveHash");
			}
		}

		static string GetDirectoryName()
		{
			try
			{
				var processes = Process.GetProcesses();
				Process process = null;
				var process1 = processes.FirstOrDefault(x => x.ProcessName == "fs_server");
				if (process1 != null)
					process = process1;
				var process2 = processes.FirstOrDefault(x => x.ProcessName == "FS_SER~1");
				if (process2 != null)
					process = process2;
				if (process != null)
				{
					var fileName = process.Modules[0].FileName;
					var fileInfo = new FileInfo(fileName);
					return fileInfo.DirectoryName;
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "DBHashHelper.GetDirectoryName");
			}
			return null;
		}
	}
}