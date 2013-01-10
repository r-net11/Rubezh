using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using Common.GK;
using FiresecAPI.XModels;
using FiresecClient;
using GKModule.Events;
using Infrastructure;
using Infrastructure.Common.Windows;
using Infrastructure.Events;
using XFiresecAPI;
using System.Threading;
using System.Diagnostics;

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
					PingJournal();
					//GetNextParameter();
				}
				catch (Exception e)
				{
					Logger.Error(e, "JournalWatcher.OnRunThread");
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