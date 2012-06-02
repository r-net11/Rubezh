using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using Infrastructure.Common.MessageBox;
using Common;

namespace Infrastructure.Common
{
	public static class SingleLaunchHelper
	{
		public static bool KillRunningProcess(bool force = false)
		{
			string processName = Process.GetCurrentProcess().ProcessName;
			bool result = true;
			Process[] processes = null;

			try
			{
				processes = Process.GetProcessesByName(processName);

				if (processes.Count() > 1)
				{
					if (!force)
						result = MessageBoxService.ShowQuestion("Другой экзэмпляр программы уже запущен. Завершить?") == MessageBoxResult.Yes;

					if (result)
					{
						foreach (var process in processes)
						{
							if (process.Id == Process.GetCurrentProcess().Id)
								continue;

							if ((process != null) && (process.HasExited == false))
							{
								process.CloseMainWindow();
								//process.WaitForExit();
								process.Kill();
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "Исключение при вызове SingleLaunchHelper.KillRunningProcess");
			}

			return result;
		}
	}

	public static class MutexHelper
	{
		static Mutex Mutex { get; set; }

		public static bool IsNew(string mutexName)
		{
			bool isNew;
			Mutex = new Mutex(true, mutexName, out isNew);
			return isNew;
		}

		public static void KeepAlive()
		{
			if (Mutex != null)
				GC.KeepAlive(Mutex);
		}
	}
}