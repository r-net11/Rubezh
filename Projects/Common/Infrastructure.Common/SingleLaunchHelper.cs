using System;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using Infrastructure.Common.MessageBox;
using System.Windows;

namespace Infrastructure.Common
{
	public static class SingleLaunchHelper
	{
		static Mutex Mutex { get; set; }

		public static bool Check(string mutexName)
		{
			return true;

			bool isNew;
			Mutex = new Mutex(true, mutexName, out isNew);
			return isNew;
		}

		public static void KeepAlive()
		{
			return;

			if (Mutex != null)
				GC.KeepAlive(Mutex);
		}

		public static bool KillRunningProcess(string processName, bool force = false)
		{
			processName = Process.GetCurrentProcess().ProcessName;
			bool result = true;
			Process[] processes = null;

			try
			{
				processes = Process.GetProcessesByName(processName);

				if (processes.Count() > 0)
				{
					Process runningProc = processes.FirstOrDefault(x => x.Id != Process.GetCurrentProcess().Id);

					if ((runningProc != null) && (runningProc.HasExited == false))
					{
						if (force == false)
							result = MessageBoxService.ShowQuestion("Другой экзэмпляр программы уже запущен. Завершить?") == MessageBoxResult.Yes;

						if (result)
						{
							runningProc.CloseMainWindow();
							//runningProc.Kill();
						}
					}
				}
			}
			finally
			{
				//if (processes != null)
				//{
				//    foreach (var process in processes)
				//    {
				//        if (process.Id != Process.GetCurrentProcess().Id)
				//            process.Dispose();
				//    }
				//}
			}

			return result;
		}
	}
}