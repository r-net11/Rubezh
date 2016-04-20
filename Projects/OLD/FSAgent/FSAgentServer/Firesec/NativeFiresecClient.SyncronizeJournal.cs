using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using Common;
using FiresecAPI;
using FiresecAPI.Models;
using System.Diagnostics;
using FSAgentAPI;
using Infrastructure.Common.Windows.BalloonTrayTip;
using FiresecDB;

namespace FSAgentServer
{
	public partial class NativeFiresecClient
	{
		int SynchrinizeJournalNo = -1;
		int SynchrinizeStepIndex = 0;
		bool ready = false;

		List<JournalRecord> GetEvents(int oldJournalNo)
		{
			var journalRecords = new List<JournalRecord>();

			var result = ReadEvents(SynchrinizeJournalNo, 100);
			if (result == null || result.Result == null || result.HasError)
			{
				OnCriticalError();
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
						if (eventId > LastJournalNo)
						{
							SynchrinizeJournalNo = eventId;
						}
						var journalRecord = JournalConverter.Convert(innerJournalItem);
						journalRecords.Add(journalRecord);
					}
				}
			}

			if (journalRecords.Count > 0)
				journalRecords = journalRecords.OrderBy(x => x.OldId).ToList();
			return journalRecords;
		}

		public bool NextStepSynchrinizeJournal()
		{
			if (ready)
				return true;

			try
			{
				if (SynchrinizeJournalNo == -1)
				{
					SynchrinizeJournalNo = DatabaseHelper.GetLastOldId();
					if (SynchrinizeJournalNo == -1)
					{
						ready = true;
						return true;
					}
				}

				var journalRecords = GetEvents(SynchrinizeJournalNo);
				journalRecords.RemoveAll(x => x.OldId < SynchrinizeJournalNo);
				if (journalRecords.Count > 0)
				{
					DatabaseHelper.AddJournalRecords(journalRecords);
				}

				SynchrinizeStepIndex++;
				if (journalRecords.Count == 0 || SynchrinizeStepIndex > 100)
				{
					ready = true;
					return true;
				}
			}
			catch (Exception e)
			{
				Logger.Error(e, "NativeFiresecClient.NextStepSynchrinizeJournal");
				ready = true;
				return true;
			}
			return false;
		}
	}
}