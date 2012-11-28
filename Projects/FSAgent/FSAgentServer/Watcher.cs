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
		FiresecSerializedClient FiresecSerializedClient;
		int LastJournalNo = 0;
		HashSet<DeviceState> ChangedDevices;
		HashSet<ZoneState> ChangedZones;
        bool MustMonitorJournal;

        public Watcher(FiresecSerializedClient firesecSerializedClient, bool mustMonitorStates, bool mustMonitorJournal)
		{
            Current = this;
			FiresecSerializedClient = firesecSerializedClient;
            MustMonitorJournal = mustMonitorJournal;
            if (mustMonitorJournal)
			{
				SetLastEvent();
            }
            if (mustMonitorStates)
            {
                firesecSerializedClient.NativeFiresecClient.NewJournalRecords += new Action<List<JournalRecord>>(OnNewJournalRecords);
				firesecSerializedClient.NativeFiresecClient.StateChanged += new Action<string>(OnStateChanged);
				firesecSerializedClient.NativeFiresecClient.ParametersChanged += new Action<string>(OnParametersChanged);
			}
			FiresecSerializedClient.NativeFiresecClient.ProgressEvent += new Func<int, string, int, int, bool>(OnProgress);
		}

		void SetLastEvent()
		{
			Firesec.Models.Journals.document journal = FiresecSerializedClient.ReadEvents(0, 100).Result;
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
				var document = FiresecSerializedClient.ReadEvents(oldJournalNo, 100).Result;
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