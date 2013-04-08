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

		public static void Close(string processName)
		{
			var processes = Process.GetProcessesByName("fs_server");
			var process = processes.FirstOrDefault();
			if (process != null)
			{
				if (process.MainWindowTitle == "Предупреждение COM Сервера" || process.MainWindowTitle == "COM Server Warning")
				{
					App.CloseOnComputerShutdown(false);
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
				Close("fs_server");
				Close("FS_SER~1");
				if (StopWatchEvent.WaitOne(5000))
					return;
			}
		}
	}
}