using System.Collections.Generic;
using System.Threading;
using Common.GK;
using Infrastructure.Common.Windows;

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
			while (true)
			{
				try
				{
                    foreach (var journalWatcher in JournalWatchers)
                    {
                        journalWatcher.Start();
                    }

					foreach (var journalWatcher in JournalWatchers)
					{
						journalWatcher.PingJournal();
					}
				}
				catch { }
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