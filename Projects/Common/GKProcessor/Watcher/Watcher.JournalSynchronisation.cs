﻿using System;
using System.Collections.Generic;
using FiresecAPI;
using FiresecAPI.GK;
using FiresecClient;
using FiresecAPI.Journal;

namespace GKProcessor
{
	public partial class Watcher
	{
		public bool ReadMissingJournalItems()
		{
			var gkIpAddress = GKManager.GetIpAddress(GkDatabase.RootDevice);
			var localLastDBNo = GKDBHelper.GetLastGKID(gkIpAddress);
			if (localLastDBNo == -1)
			{
				return true;
			}
			var remoteLastId = GetLastId();
			if (remoteLastId == -1)
			{
				return false;
			}
			if (remoteLastId > localLastDBNo)
			{
				var progressCallback = GKProcessorManager.StartProgress("Синхронизация журнала ГК " + gkIpAddress, "", remoteLastId - localLastDBNo, true, GKProgressClientType.Monitor);

				var journalItems = new List<JournalItem>();
				for (int index = localLastDBNo; index <= remoteLastId; index++)
				{
					LastUpdateTime = DateTime.Now;
					if (progressCallback.IsCanceled)
						break;

					WaitIfSuspending();
					if (IsStopping)
					{
						break;
					}

					var journaParser = ReadJournal(index);
					if (journaParser != null)
					{
						GKProcessorManager.DoProgress((index - localLastDBNo).ToString() + " из " + (remoteLastId - localLastDBNo).ToString(), progressCallback);

						journalItems.Add(journaParser.JournalItem);
						if (journalItems.Count > 100)
						{
							AddJournalItems(journalItems);
							journalItems = new List<JournalItem>();
						}
					}
				}
				if (journalItems.Count > 0)
				{
					AddJournalItems(journalItems);
				}

				GKProcessorManager.StopProgress(progressCallback);
				LastId = remoteLastId;
			}
			return true;
		}
	}
}