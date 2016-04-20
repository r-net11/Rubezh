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
		static int LastJournalNo = 0;

		void SetLastEvent()
		{
			try
			{
				var result = ReadEvents(0, 100);
				if (result == null || result.Result == null || result.HasError)
				{
					OnCriticalError();
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

				var result = ReadEvents(LastJournalNo, 100);
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
								LastJournalNo = eventId;
							}
							var journalRecord = JournalConverter.Convert(innerJournalItem);
							journalRecords.Add(journalRecord);
							hasNewRecords = true;
						}
					}
				}
				if (!hasNewRecords)
					break;
			}

			if (journalRecords.Count > 0)
				journalRecords = journalRecords.OrderBy(x => x.OldId).ToList();
			return journalRecords;
		}
	}
}