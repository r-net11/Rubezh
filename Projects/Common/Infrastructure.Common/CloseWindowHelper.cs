using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;

namespace Infrastructure.Common
{
	public static class CloseWindowHelper
	{
		[DllImport("user32.dll")]
		public static extern int FindWindow(string lpClassName, string lpWindowName);
		[DllImport("user32.dll")]
		public static extern int SendMessage(int hWnd, uint Msg, int wParam, int lParam);

		const int WM_SYSCOMMAND = 0x0112;
		const int SC_CLOSE = 0xF060;

		public static void Close()
		{
			var processes = Process.GetProcessesByName("fs_server");
			var process = processes.FirstOrDefault();
			if (process != null)
			{
				if (process.MainWindowTitle == "Предупреждение COM Сервера")
				{
					process.Kill();
				}
			}
		}

		static void Close2()
		{
			int iHandle = FindWindow("fs_server.exe", "Предупреждение COM Сервера");
			int iHandle2 = FindWindow("fs_server", "Предупреждение COM Сервера");
			if (iHandle > 0)
			{
				SendMessage(iHandle, WM_SYSCOMMAND, SC_CLOSE, 0);
			}
		}

		static AutoResetEvent StopWatchEvent;
		static Thread WatchThread;

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
				if (StopWatchEvent.WaitOne(5000))
					return;
			}
		}
	}
}