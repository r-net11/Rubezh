using System.Collections.Generic;
using System.Threading;
using Common;
using Common.GK;
using Infrastructure.Common.Windows;
using System;

namespace GKModule
{
	public static class JournalWatcherManager
	{
		static Thread thread;
		static AutoResetEvent StopEvent;
		static List<JournalWatcher> JournalWatchers;

		public static void Start()
		{
			JournalWatchers = new List<JournalWatcher>();
			foreach (var gkDatabase in DatabaseManager.GkDatabases)
			{
				var ipAddress = gkDatabase.RootDevice.GetGKIpAddress();
				if (ipAddress == null)
				{
					Logger.Error("JournalWatcherManager.Start ipAddress = null");
					continue;
				}
				var journalWatcher = new JournalWatcher(gkDatabase);
				JournalWatchers.Add(journalWatcher);
			}

			StopEvent = new AutoResetEvent(false);
			thread = new Thread(new ThreadStart(OnRun));
			thread.Start();
			ApplicationService.Closing += new System.ComponentModel.CancelEventHandler(ApplicationService_Closing);
		}

		static void ApplicationService_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			StopEvent.Set();
		}

		static void OnRun()
		{
			try
			{
				foreach (var journalWatcher in JournalWatchers)
				{
					journalWatcher.Start();
				}
			}
			catch(Exception e)
			{
				Logger.Error(e, "JournalWatcherManager.OnRun 1");
			}

			while (true)
			{
				try
				{
					foreach (var journalWatcher in JournalWatchers)
					{
						journalWatcher.PingJournal();
					}
				}
				catch (Exception e)
				{
					Logger.Error(e, "JournalWatcherManager.OnRun 2");
				}
				if (StopEvent.WaitOne(100))
					break;
			}
		}

		public static void GetLastJournalItems(int count)
		{
            var thread = new Thread(new ThreadStart(
                () => {
                    foreach (var journalWatcher in JournalWatchers)
                    {
                        journalWatcher.GetLastJournalItems(count);
                    }
                }
                ));
            thread.Start();
		}
	}
}