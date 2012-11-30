using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Firesec;
using FiresecAPI;
using System.Threading;
using System.Diagnostics;
using FiresecAPI.Models;
using System.Windows.Threading;
using Common;
using FSAgentAPI;

namespace FSAgentServer
{
    public partial class WatcherManager
	{
		public static WatcherManager Current { get; private set; }
        AutoResetEvent StopEvent = new AutoResetEvent(false);
        Thread RunThread;
		public NativeFiresecClient DirectClient { get; private set; }
		NativeFiresecClient CallbackClient;
        int PollIndex = 0;
		public static AutoResetEvent PoolSleepEvent = new AutoResetEvent(false);
		public FSProgressInfo LastFSProgressInfo { get; set; }

		public WatcherManager()
		{
			Current = this;
		}

        public void Start()
        {
            if (RunThread == null)
            {
				CallbackClient = new NativeFiresecClient();
                CallbackClient.Connect();
				CallbackClient.IsPing = true;

                StopEvent = new AutoResetEvent(false);
                RunThread = new Thread(OnRun);
                RunThread.SetApartmentState(ApartmentState.STA);
                RunThread.IsBackground = true;
                RunThread.Start();

                StartLifetimeThread();
            }
        }

        public void Stop()
        {
            StopLifetimeThread();

            if (StopEvent != null)
            {
                StopEvent.Set();
            }
            if (RunThread != null)
            {
                RunThread.Join(TimeSpan.FromSeconds(1));
            }
        }

		public static void WaikeOnEvent()
		{
			PoolSleepEvent.Set();
		}

		void OnRun()
		{
			try
			{
				DirectClient = new NativeFiresecClient();
				DirectClient.Connect();
				DirectClient.ProgressEvent += new Action<int, string, int, int>(OnAdministratorProgress);
			}
			catch (Exception e)
			{
				Logger.Error(e, "OnRun");
			}

			while (true)
			{
				try
				{
					PoolSleepEvent = new AutoResetEvent(false);
					PoolSleepEvent.WaitOne(TimeSpan.FromSeconds(1));
                    PollIndex++;
                    var force = PollIndex % 100 == 0;

                    try
                    {
						CheckNonBlockingTasks();
						CheckBlockingTasks();
                        DirectClient.CheckForRead(force);
                    }
                    catch (Exception e)
                    {
                        Logger.Error(e, "OnRun.while");
                    }
				}
				catch (Exception e)
				{
					Logger.Error(e, "OnRun.while2");
				}
			}
		}

		public event Action<int, string, int, int> Progress;
		void OnProgress(int stage, string comment, int percentComplete, int bytesRW)
		{
			if (Progress != null)
				Progress(stage, comment, percentComplete, bytesRW);
		}

		void OnAdministratorProgress(int stage, string comment, int percentComplete, int bytesRW)
		{
			LastFSProgressInfo = new FSProgressInfo()
			{
				Stage = stage,
				Comment = comment,
				PercentComplete = percentComplete,
				BytesRW = bytesRW
			};
			ClientsManager.ClientInfos.ForEach(x => x.PollWaitEvent.Set());
		}
	}
}