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
        public static event Action<JournalRecord> NewJournalRecordEvent;
        public static event Action ConfigurationChangedEvent;
        public static event Action<IEnumerable<JournalRecord>> GetFilteredArchiveCompletedEvent;

        static void SafeOperationCall(Action action)
        {
            try
            {
                action();
            }
            catch {}
        }

		bool isConnected = true;
		static AutoResetEvent StopPingEvent;
		static Thread pingThread;
		static bool suspendPing = false;

		public void StartPing()
		{
			StopPingEvent = new AutoResetEvent(false);
			pingThread = new Thread(new ThreadStart(OnPing));
			pingThread.Start();
		}

		public void StopPing()
		{
			if (StopPingEvent != null)
			{
				StopPingEvent.Set();
			}
		}

        void OnPing()
        {
            IAsyncResult asyncResult = BeginPoll(0, DateTime.Now, new AsyncCallback(CallbackPall), (IFiresecService)this);
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
                    BeginPoll(0, DateTime.Now, new AsyncCallback(CallbackPall), (IFiresecService)this);
                }
            }
        }

		public string Ping()
		{
			try
			{
				var result = FiresecService.Ping();
				OnConnectionAppeared();
				return result;
			}
			catch
			{
				OnConnectionLost();
				Recover();
			}
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
			suspendPing = true;
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
				suspendPing = false;
			}
		}
	}
}