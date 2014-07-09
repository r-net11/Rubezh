using System;
using System.Threading;
using Common;
using FiresecAPI.GK;
using FiresecAPI.Journal;

namespace GKProcessor
{
	internal static class LifeTimeWatcher
	{
		static Thread Thread;
		static AutoResetEvent AutoResetEvent = new AutoResetEvent(false);

		public static void Start()
		{
			if (Thread == null)
			{
				Thread = new Thread(OnRun);
				Thread.Name = "GK LifeTimeWatcher";
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
							if (watcher.IsStopping || watcher.IsSuspending)
							{
								watcher.AddMessage(JournalEventNameType.Зависание_процесса_отпроса, watcher.GkDatabase.RootDevice.PredefinedName);
							}
							else
							{
								Logger.Error("LifeTimeWatcher.OnRun watcher");
								watcher.AddMessage(JournalEventNameType.Зависание_процесса_отпроса, watcher.GkDatabase.RootDevice.PredefinedName);
								watcher.ConnectionChanged(false);
								watcher.StopThread();
								watcher.StartThread();
							}
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