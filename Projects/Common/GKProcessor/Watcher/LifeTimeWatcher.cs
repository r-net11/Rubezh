using System;
using System.Threading;
using Common;

namespace GKProcessor
{
	public static class LifeTimeWatcher
	{
		static Thread Thread;
		static AutoResetEvent AutoResetEvent = new AutoResetEvent(false);

		public static void Start()
		{
			if (Thread == null)
			{
				Thread = new Thread(OnRun);
				Thread.Start();
			}
		}

		public static void Stop()
		{
			if (AutoResetEvent != null)
			{
				AutoResetEvent.Set();
				if (Thread != null)
				{
					Thread.Join(TimeSpan.FromSeconds(1));
				}
			}
		}

		static void OnRun()
		{
			while (true)
			{
				try
				{
					AutoResetEvent = new AutoResetEvent(false);
					if (AutoResetEvent.WaitOne(TimeSpan.FromMinutes(1)))
					{
						break;
					}

					foreach (var watcher in WatcherManager.Watchers)
					{
						if ((DateTime.Now - watcher.LastUpdateTime).TotalMinutes > 5)
						{
							Logger.Error("LifeTimeWatcher.OnRun watcher");
							GKDBHelper.AddMessage("Зависание процесса отпроса", "");
							watcher.ConnectionChanged(false);
							watcher.StopThread();
							watcher.StartThread();
						}
					}
				}
				catch (Exception e)
				{
					Logger.Error(e, "LifeTimeWatcher.OnRun");
				}
			}
		}
	}
}