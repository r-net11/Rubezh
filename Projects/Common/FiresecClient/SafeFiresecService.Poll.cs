using System;
using System.Threading;
using FiresecAPI.Models;
using FiresecAPI;
using System.Diagnostics;
using System.Collections.Generic;

namespace FiresecClient
{
	public partial class SafeFiresecService
	{
        static void SafeOperationCall(Action action)
        {
            try
            {
                action();
            }
            catch { }
        }

        public static event Action<JournalRecord> NewJournalRecordEvent;
        public static event Action ConfigurationChangedEvent;
        public static event Action<IEnumerable<JournalRecord>> GetFilteredArchiveCompletedEvent;

		bool isConnected = true;
		static AutoResetEvent StopPollEvent;
		static bool suspendPoll = false;

		public void StopPoll()
		{
			if (StopPollEvent != null)
			{
				StopPollEvent.Set();
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
                                    var thread = new Thread(new ThreadStart(() =>
                                    {
                                        if (ConfigurationChangedEvent != null)
                                            ConfigurationChangedEvent();
                                    }));
                                    thread.Start();
                                });
                                break;
                        }
                    }
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
			suspendPoll = true;
			FiresecServiceFactory.Dispose();
			FiresecServiceFactory = new FiresecClient.FiresecServiceFactory();
			FiresecService = FiresecServiceFactory.Create(_serverAddress);
			try
			{
				FiresecService.Connect(_clientCredentials, false);
				return true;
			}
			catch
			{
				return false;
			}
			finally
			{
				suspendPoll = false;
			}
		}
	}
}