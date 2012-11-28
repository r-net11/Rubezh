using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using FSAgentAPI;

namespace FSAgentServer
{
	public class Watcher
	{
        internal static Watcher Current;
		NativeFiresecClient NativeFiresecClient;
		int LastJournalNo = 0;
		HashSet<DeviceState> ChangedDevices;
		HashSet<ZoneState> ChangedZones;
        bool MustMonitorJournal;

		public Watcher(NativeFiresecClient nativeFiresecClient, bool mustMonitorStates, bool mustMonitorJournal)
		{
            Current = this;
			NativeFiresecClient = nativeFiresecClient;
            MustMonitorJournal = mustMonitorJournal;
            if (mustMonitorJournal)
			{
				SetLastEvent();
            }
            if (mustMonitorStates)
            {
                nativeFiresecClient.NewJournalRecords += new Action<List<JournalRecord>>(OnNewJournalRecords);
				nativeFiresecClient.StateChanged += new Action<string>(OnStateChanged);
				nativeFiresecClient.ParametersChanged += new Action<string>(OnParametersChanged);
			}
			NativeFiresecClient.ProgressEvent += new Func<int, string, int, int, bool>(OnProgress);
		}

		OperationResult<T> ConvertResultData<T>(OperationResult<string> result)
		{
			var resultData = new OperationResult<T>()
			{
				HasError = result.HasError,
				Error = result.Error
			};
			if (result.HasError == false)
				resultData.Result = SerializerHelper.Deserialize<T>(result.Result);
			return resultData;
		}

		void SetLastEvent()
		{
			var result = NativeFiresecClient.ReadEvents(0, 100);
			if (result.HasError)
			{
				Logger.Error("FSAgentServer.SetLastEvent " + result.Error);
				return;
			}
			var convertResult = ConvertResultData<Firesec.Models.Journals.document>(result);
			if (convertResult.HasError && convertResult.Result == null)
			{
				Logger.Error("FSAgentServer.SetLastEvent " + convertResult.Error);
				return;
			}
			Firesec.Models.Journals.document journal = convertResult.Result;
			if (journal != null && journal.Journal.IsNotNullOrEmpty())
			{
				foreach (var journalItem in journal.Journal)
				{
					var intValue = int.Parse(journalItem.IDEvents);
					if (intValue > LastJournalNo)
						LastJournalNo = intValue;
				}
			}
		}

		public List<JournalRecord> SynchrinizeJournal(int oldJournalNo)
		{
			if (oldJournalNo >= 0)
			{
				var journalRecords = GetEventsFromLastId(oldJournalNo);
				return journalRecords;
			}
			return new List<JournalRecord>();
		}

		List<JournalRecord> GetEventsFromLastId(int oldJournalNo)
		{
			var result = new List<JournalRecord>();

			var hasNewRecords = true;
			for (int i = 0; i < 100; i++ )
			{
				hasNewRecords = false;

				var readResult = NativeFiresecClient.ReadEvents(0, 100);
				if (readResult.HasError)
				{
					Logger.Error("FSAgentServer.SetLastEvent " + readResult.Error);
					return new List<JournalRecord>();
				}
				var convertResult = ConvertResultData<Firesec.Models.Journals.document>(readResult);
				if (convertResult.HasError && convertResult.Result == null)
				{
					Logger.Error("FSAgentServer.SetLastEvent " + convertResult.Error);
					return new List<JournalRecord>();
				}
				Firesec.Models.Journals.document document = convertResult.Result;

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

		public void OnParametersChanged(string deviceParameters)
		{
			CallbackManager.Add(new FSAgentCallbac() { CoreDeviceParams = deviceParameters });
		}

		void OnStateChanged(string deviceStates)
		{
			CallbackManager.Add(new FSAgentCallbac() { CoreCongig = deviceStates });
		}

		void OnNewJournalRecords(List<JournalRecord> journalRecords)
		{
            CallbackManager.Add(new FSAgentCallbac() { JournalRecords = journalRecords });
		}

        public event Func<int, string, int, int, bool> Progress;
		bool OnProgress(int stage, string comment, int percentComplete, int bytesRW)
		{
			if (Progress != null)
				return Progress(stage, comment, percentComplete, bytesRW);
            return true;
		}
	}
}