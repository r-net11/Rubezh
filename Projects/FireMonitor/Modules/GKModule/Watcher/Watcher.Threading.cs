using System;
using System.Threading;
using Common;
using Common.GK;

namespace GKModule
{
	public partial class Watcher
	{
		public bool IsStopping = false;
		AutoResetEvent StopEvent;
		Thread RunThread;
		public GkDatabase GkDatabase { get; private set; }

		public Watcher(GkDatabase gkDatabase)
		{
			GkDatabase = gkDatabase;
		}

		public void StartThread()
		{
			if (RunThread == null)
			{
				StopEvent = new AutoResetEvent(false);
				RunThread = new Thread(OnRunThread);
				RunThread.Start();
			}
		}

		public void StopThread()
		{
			IsStopping = true;

			if (StopEvent != null)
			{
				StopEvent.Set();
			}
			if (RunThread != null)
			{
				RunThread.Join(TimeSpan.FromSeconds(1));
			}
		}

		void OnRunThread()
		{
            GetAllStates();

			while (true)
			{
				try
				{
					CheckTasks();
				}
				catch (Exception e)
				{
					Logger.Error(e, "JournalWatcher.OnRunThread CheckTasks");
				}

				try
				{
					CheckNPT();
				}
				catch (Exception e)
				{
					Logger.Error(e, "JournalWatcher.OnRunThread CheckNPT");
				}

				try
				{
					PingJournal();
				}
				catch (Exception e)
				{
					Logger.Error(e, "JournalWatcher.OnRunThread PingJournal");
				}

				if (StopEvent != null)
				{
					if (StopEvent.WaitOne(10))
						break;
				}
			}
		}
	}
}