using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;

namespace FSAgentServer
{
	public static class ComServerExceptionWatcher
	{
		static AutoResetEvent StopWatchEvent;
		static Thread WatchThread;

		public static void Close()
		{
			var processes = Process.GetProcessesByName("fs_server");
			var process = processes.FirstOrDefault();
			if (process != null)
			{
				if (process.MainWindowTitle == "Предупреждение COM Сервера")
				{
					App.CloseOnComputerShutdown();
					process.Kill();
				}
			}
		}

		public static void Close2()
		{
			var processes = Process.GetProcessesByName("FS_SER~1");
			var process = processes.FirstOrDefault();
			if (process != null)
			{
				if (process.MainWindowTitle == "Предупреждение COM Сервера")
				{
					App.CloseOnComputerShutdown();
					process.Kill();
				}
			}
		}

		public static void StartWatchThread()
		{
			if (WatchThread == null)
			{
				StopWatchEvent = new AutoResetEvent(false);
				WatchThread = new Thread(OnWatch);
				WatchThread.Start();
			}
		}

		public static void StopWatchThread()
		{
			if (StopWatchEvent != null)
			{
				StopWatchEvent.Set();
			}
			if (WatchThread != null)
			{
				WatchThread.Join(TimeSpan.FromSeconds(1));
			}
		}

		static void OnWatch()
		{
			while (true)
			{
				Close();
				Close2();
				if (StopWatchEvent.WaitOne(5000))
					return;
			}
		}
	}
}