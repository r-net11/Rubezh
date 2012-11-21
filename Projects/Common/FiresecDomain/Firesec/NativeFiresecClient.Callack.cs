using System;
using System.Collections.Generic;
using System.Threading;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using System.Diagnostics;
using FiresecDomain;

namespace FiresecDomain
{
    public partial class NativeFiresecClient
    {
        public static bool IsSuspended { get; set; }
		int LastJournalNo = 0;
		static bool needToRead = false;
		static bool needToReadStates = false;
		static bool needToReadParameters = false;
		static bool needToReadJournal = false;

		public void NewEventsAvailable(int eventMask)
		{
			if (IsPing)
			{
				needToRead = true;
				needToReadJournal = ((eventMask & 1) == 1);
				needToReadStates = ((eventMask & 2) == 2);
				needToReadParameters = ((eventMask & 8) == 8);
				DomainRunner.MonitoringNativeFiresecClient.CheckForRead();
				//FiresecDriver.MonitoringFiresecSerializedClient.NativeFiresecClient.CheckForRead();
			}
		}

		public void CheckForRead()
		{
            if (IsSuspended)
                return;

			if (needToRead)
			{
				needToRead = false;

				SuspendOperationQueueEvent = new AutoResetEvent(false);
				try
				{
					if (needToReadStates)
					{
						needToReadStates = false;
						var result = SafeCall<string>(() => { return ReadFromStream(Connection.GetCoreState()); }, "GetCoreState");
						if (result != null && result.Result != null)
						{
							var coreState = ConvertResultData<Firesec.Models.CoreState.config>(result);
							if (coreState.Result != null)
							{
								if (StateChanged != null)
									StateChanged(coreState.Result);
							}
						}
					}

					if (needToReadParameters)
					{
						needToReadParameters = false;
						var result = SafeCall<string>(() => { return Connection.GetCoreDeviceParams(); }, "GetCoreDeviceParams");
						if (result != null && result.Result != null)
						{
							var coreParameters = ConvertResultData<Firesec.Models.DeviceParameters.config>(result);
							if (coreParameters.Result != null)
							{
								if (ParametersChanged != null)
									ParametersChanged(coreParameters.Result);
							}
						}
					}

					if (needToReadJournal)
					{
						needToReadJournal = false;
						var journalRecords = GetEventsFromLastId(LastJournalNo);

						if (NewJournalRecords != null)
							NewJournalRecords(journalRecords);
					}
				}
				catch (Exception e)
				{
					Logger.Error(e, "NativeFiresecClient.NewEventsAvailable");
				}
				finally
				{
					if (SuspendOperationQueueEvent != null)
					{
						SuspendOperationQueueEvent.Set();
					}
				}
			}
		}

		void SetLastEvent()
		{
			try
			{
				var result = ReadEvents(0, 100);
				if (result.HasError)
				{
					LastJournalNo = 0;
				}
				var document = SerializerHelper.Deserialize<Firesec.Models.Journals.document>(result.Result);
				if (document != null && document.Journal.IsNotNullOrEmpty())
				{
					foreach (var journalItem in document.Journal)
					{
						var intValue = int.Parse(journalItem.IDEvents);
						if (intValue > LastJournalNo)
							LastJournalNo = intValue;
					}
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "NativeFiresecClient.SetLastEvent");
			}
		}

		List<JournalRecord> GetEventsFromLastId(int oldJournalNo)
		{
			var journalRecords = new List<JournalRecord>();

			var hasNewRecords = true;
			for (int i = 0; i < 100; i++)
			{
				hasNewRecords = false;

				var result = ReadEvents(oldJournalNo, 100);
				if (result == null || result.HasError)
				{
					return new List<JournalRecord>();
				}

				var document = SerializerHelper.Deserialize<Firesec.Models.Journals.document>(result.Result);
				if (document != null && document.Journal.IsNotNullOrEmpty())
				{
					foreach (var innerJournalItem in document.Journal)
					{
						var eventId = int.Parse(innerJournalItem.IDEvents);
						if (eventId > oldJournalNo)
						{
							LastJournalNo = eventId;
							oldJournalNo = eventId;
							var journalRecord = JournalConverter.Convert(innerJournalItem);
							journalRecords.Add(journalRecord);
							hasNewRecords = true;
						}
					}
				}
				if (!hasNewRecords)
					break;
			}

			return journalRecords;
		}

        OperationResult<T> ConvertResultData<T>(OperationResult<string> result)
        {
            var resultData = new OperationResult<T>();
            resultData.HasError = result.HasError;
            resultData.Error = result.Error;
            if (result.HasError == false)
                resultData.Result = SerializerHelper.Deserialize<T>(result.Result);
            return resultData;
        }

        public bool Progress(int Stage, string Comment, int PercentComplete, int BytesRW)
        {
            if (IsSuspended)
                return true;

            try
            {
                if (ProgressEvent != null)
                {
                    return ProgressEvent(Stage, Comment, PercentComplete, BytesRW);
                }
                return true;
            }
            catch (Exception e)
            {
                Logger.Error(e, "Исключение при вызове NativeFiresecClient.Progress");
                return false;
            }
        }

		public event Action<List<JournalRecord>> NewJournalRecords;
        public event Action<Firesec.Models.CoreState.config> StateChanged;
        public event Action<Firesec.Models.DeviceParameters.config> ParametersChanged;
        public event Func<int, string, int, int, bool> ProgressEvent;
    }
}