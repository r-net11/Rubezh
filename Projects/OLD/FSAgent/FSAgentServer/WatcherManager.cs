using System;
using System.Threading;
using Common;
using FSAgentServer.ViewModels;
using Infrastructure.Common.Windows.BalloonTrayTip;

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

		public WatcherManager()
		{
			Current = this;
		}

		public void Start()
		{
			StartRunThread();
			StartLifetimeThread();
			ComServerExceptionWatcher.StartWatchThread();
		}

		public void Stop()
		{
			StopLifetimeThread();
			StopRunThread();
			ComServerExceptionWatcher.StopWatchThread();
		}

		public void StartRunThread()
		{
			if (RunThread == null)
			{
				StopEvent = new AutoResetEvent(false);
				RunThread = new Thread(OnRun);
				RunThread.Name = "WatcherManager Run";
				RunThread.SetApartmentState(ApartmentState.STA);
				RunThread.IsBackground = true;
				RunThread.Start();
			}
		}

		public void StopRunThread()
		{
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
				UILogger.Log("Запуск драйвера для администрирования");
				CallbackClient = new NativeFiresecClient();
				var connectResult1 = CallbackClient.Connect();
				if (connectResult1.HasError)
				{
					UILogger.Log("Ошибка соединения с драйвером для администрирования");
					BalloonHelper.ShowFromAgent("Ошибка соединения с драйвером для администрирования");
				}
				CallbackClient.IsPing = true;

				UILogger.Log("Запуск драйвера для мониторинга");
				DirectClient = new NativeFiresecClient();
				var connectResult2 = DirectClient.Connect();
				if (connectResult2.HasError)
				{
					UILogger.Log("Ошибка соединения с драйвером для мониторинга");
					BalloonHelper.ShowFromAgent("Ошибка соединения с драйвером для мониторинга");
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
					CircleDateTime = DateTime.Now;
					PoolSleepEvent = new AutoResetEvent(false);
					if (DirectClient.NextStepSynchrinizeJournal())
					{
						PoolSleepEvent.WaitOne(TimeSpan.FromSeconds(1));
					}
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
	}
}