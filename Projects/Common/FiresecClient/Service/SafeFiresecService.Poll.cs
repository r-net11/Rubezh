using System;
using System.Collections.Generic;
using System.Threading;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using Infrastructure.Common;

namespace FiresecClient
{
	public partial class SafeFiresecService
	{
		public static event Action<JournalRecord> NewJournalRecordEvent;
		public static event Action ConfigurationChangedEvent;
		public static event Action<IEnumerable<JournalRecord>> GetFilteredArchiveCompletedEvent;

		bool isConnected = true;
		static AutoResetEvent StopPollEvent;
		public bool SuspendPoll = false;
		Thread ShortPollThread;
		bool IsLongPollPeriod;

		public void StopPoll()
		{
			if (StopPollEvent != null)
			{
				StopPollEvent.Set();
			}
			if (ShortPollThread != null)
			{
				ShortPollThread.Join(2000);
			}
		}

		public void StartShortPoll(bool isLongPollPeriod)
		{
			IsLongPollPeriod = isLongPollPeriod;
			StopPollEvent = new AutoResetEvent(false);
			ShortPollThread = new Thread(OnShortPoll);
			ShortPollThread.IsBackground = true;
			ShortPollThread.Start();
		}

		void OnShortPoll()
		{
			while (true)
			{
				try
				{
					int sleepInterval = IsLongPollPeriod ? 60000 : 1000;
					if (!StopPollEvent.WaitOne(sleepInterval))
					{
                        if (SuspendPoll)
                            continue;

						var callbackResults = ShortPoll();
						if (!IsLongPollPeriod)
						{
							ProcessCallbackResult(callbackResults);
						}
					}
					else
					{
						return;
					}
				}
				catch (Exception e)
				{
					Logger.Error(e, "SafeFiresecService.OnPoll");
				}
			}
		}

		public void StartPoll()
		{
			try
			{
				BeginPoll(0, DateTime.Now, new AsyncCallback(CallbackPall), (IFiresecService)this);
			}
			catch
			{
				OnConnectionLost();
				Recover();

				StopPollEvent = new AutoResetEvent(false);
				if (StopPollEvent.WaitOne(5000))
				{
					if (!FiresecManager.IsDisconnected)
					{
						StartPoll();
					}
				}
			}
		}

		void CallbackPall(IAsyncResult asyncResult)
		{
			try
			{
				IFiresecService firesecService = asyncResult.AsyncState as IFiresecService;
				if (firesecService != null)
				{
					List<CallbackResult> callbackResults = firesecService.EndPoll(asyncResult);
					ProcessCallbackResult(callbackResults);
				}
			}
			catch (Exception)
			{
				;
			}
			finally
			{
				if (!FiresecManager.IsDisconnected)
				{
					StartPoll();
				}
			}
		}

		void ProcessCallbackResult(List<CallbackResult> callbackResults)
		{
			if (callbackResults == null || callbackResults.Count == 0)
				return;

			foreach (var callbackResult in callbackResults)
			{
				switch (callbackResult.CallbackResultType)
				{
					case CallbackResultType.NewEvents:
						foreach (var journalRecord in callbackResult.JournalRecords)
						{
							SafeOperationCall(() =>
							{
								if (NewJournalRecordEvent != null)
									NewJournalRecordEvent(journalRecord);
							});
						}
						break;

					case CallbackResultType.ArchiveCompleted:
						SafeOperationCall(() =>
						{
							if (GetFilteredArchiveCompletedEvent != null)
								GetFilteredArchiveCompletedEvent(callbackResult.JournalRecords);
						});
						break;

					case CallbackResultType.ConfigurationChanged:
						SafeOperationCall(() =>
						{
                            if (ConfigurationChangedEvent != null)
                                ConfigurationChangedEvent();
						});
						break;
				}
			}
		}

		public string Ping()
		{
			return null;
		}

		public static event Action ConnectionLost;
		void OnConnectionLost()
		{
			if (isConnected == false)
				return;
			if (ConnectionLost != null)
				ConnectionLost();
			isConnected = false;
            ServerLoadHelper.Load();
		}

		public static event Action ConnectionAppeared;
		void OnConnectionAppeared()
		{
			if (isConnected == true)
				return;

			if (ConnectionAppeared != null)
				ConnectionAppeared();

			isConnected = true;
		}

		bool Recover()
		{
			Logger.Error("SafeFiresecService.Recover");

			SuspendPoll = true;
			try
			{
				FiresecServiceFactory.Dispose();
				FiresecServiceFactory = new FiresecClient.FiresecServiceFactory();
				FiresecService = FiresecServiceFactory.Create(_serverAddress);
				FiresecService.Connect(_clientCredentials, false);
				return true;
			}
			catch
			{
				return false;
			}
			finally
			{
				SuspendPoll = false;
			}
		}

        static void SafeOperationCall(Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                Logger.Error(e, "SafeFiresecService.SafeOperationCall");
            }
        }
	}
}