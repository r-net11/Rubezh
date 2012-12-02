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
using FSAgentServer.ViewModels;
using Infrastructure.Common.BalloonTrayTip;

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
                UILogger.Log("Запуск драйвера для администрирования");
				CallbackClient = new NativeFiresecClient();
                var connectResult = CallbackClient.Connect();
                if (connectResult.HasError)
                {
                    UILogger.Log("Ошибка соединения с драйвером для администрирования");
                    BalloonHelper.ShowWarning("Агент Firesec", "Ошибка соединения с драйвером для администрирования");
                }
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

            if (DirectClient != null)
                DirectClient.Close();
            if (CallbackClient != null)
                CallbackClient.Close();
        }

		public static void WaikeOnEvent()
		{
			PoolSleepEvent.Set();
		}

		void OnRun()
		{
			try
			{
                UILogger.Log("Запуск драйвера для мониторинга");
				DirectClient = new NativeFiresecClient();
				var connectResult = DirectClient.Connect();
                if (connectResult.HasError)
                {
                    UILogger.Log("Ошибка соединения с драйвером для мониторинга");
                    BalloonHelper.ShowWarning("Агент Firesec", "Ошибка соединения с драйвером для мониторинга");
                }
                Bootstrapper.BootstrapperLoadEvent.Set();
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

                    try
                    {
						CheckNonBlockingTasks();
						CheckBlockingTasks();

                        var force = PollIndex % 100 == 0;
                        if (PollIndex % 200 == 0)
                        {
                            CallbackClient.CheckForRead(true);
                        }
                        else
                        {
                            DirectClient.CheckForRead(force);
                        }
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

		public void OnAdministratorProgress(int stage, string comment, int percentComplete, int bytesRW)
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