using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Threading;
using Common;
using FiresecAPI;
using System.Diagnostics;
using FiresecAPI.Models;

namespace Firesec
{
    public partial class NativeFiresecClient
    {
        public void NewEventsAvailable(int eventMask)
        {
			if (!IsPing)
				return;

			needToRead = true;

            bool evmNewEvents = ((eventMask & 1) == 1);
            bool evmStateChanged = ((eventMask & 2) == 2);
            bool evmConfigChanged = ((eventMask & 4) == 4);
            bool evmDeviceParamsUpdated = ((eventMask & 8) == 8);
            bool evmPong = ((eventMask & 16) == 16);
            bool evmDatabaseChanged = ((eventMask & 32) == 32);
            bool evmReportsChanged = ((eventMask & 64) == 64);
            bool evmSoundsChanged = ((eventMask & 128) == 128);
            bool evmLibraryChanged = ((eventMask & 256) == 256);
            bool evmPing = ((eventMask & 512) == 512);
            bool evmIgnoreListChanged = ((eventMask & 1024) == 1024);
            bool evmEventViewChanged = ((eventMask & 2048) == 2048);

			if (evmStateChanged)
			{
				needToReadStates = true;
			}
			if (needToReadParameters)
			{
				needToReadParameters = true;
			}
			if (evmNewEvents)
			{
				needToReadJournal = true;
			}

			CheckForRead();
        }

		bool needToRead = false;
		bool needToReadStates = false;
		bool needToReadParameters = false;
		bool needToReadJournal = false;

		void CheckForRead()
		{
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

						if (NewJournalRecord != null)
							NewJournalRecord(journalRecords);
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
					SuspendOperationQueueEvent = null;
				}
			}
		}

		int LastJournalNo = 0;

		void SetLastEvent()
		{
			var result1 = ReadEvents(0, 100);
			if (result1.HasError)
			{
				;
			}
			var document = SerializerHelper.Deserialize<Firesec.Models.Journals.document>(result1.Result);

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

		List<JournalRecord> GetEventsFromLastId(int oldJournalNo)
		{
			var result = new List<JournalRecord>();

			var hasNewRecords = true;
			for (int i = 0; i < 100; i++)
			{
				hasNewRecords = false;

				var result1 = ReadEvents(oldJournalNo, 100);
				if (result1.HasError)
				{
					;
				}
				var document = SerializerHelper.Deserialize<Firesec.Models.Journals.document>(result1.Result);

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
							result.Add(journalRecord);
							hasNewRecords = true;
						}
					}
				}
				if (!hasNewRecords)
					break;
			}

			return result;
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

		public event Action<List<JournalRecord>> NewJournalRecord;
        public event Action<Firesec.Models.CoreState.config> StateChanged;
        public event Action<Firesec.Models.DeviceParameters.config> ParametersChanged;
        public event Action<int> NewEventAvaliable;
        public event Func<int, string, int, int, bool> ProgressEvent;
    }
}