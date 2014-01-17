﻿using System.Collections.Generic;
using FiresecClient;
using System.Threading;
using Infrastructure.Common.Windows;
using XFiresecAPI;
using Common;

namespace GKProcessor
{
	public partial class Watcher
	{
		public bool ReadMissingJournalItems()
		{
			var gkIpAddress = XManager.GetIpAddress(GkDatabase.RootDevice);
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
				GKProcessorManager.OnStartProgress("Синхронизация журнала ГК " + gkIpAddress, "", remoteLastId - localLastDBNo, true);
				SyncLocalAndRemote(localLastDBNo, remoteLastId);
				GKProcessorManager.OnStopProgress();
				LastId = remoteLastId;
			}
			return true;
		}

		void SyncLocalAndRemote(int startIndex, int endIndex)
		{
			var journalItems = new List<JournalItem>();
			for (int index = startIndex; index <= endIndex; index++)
			{
				if (GKProcessorManager.IsProgressCanceled)
					break;

				WaitIfSuspending();
				if (IsStopping)
					return;

				var journalItem = ReadJournal(index);
				if (journalItem != null)
				{
					GKProcessorManager.OnDoProgress((index - startIndex).ToString() + " из " + (endIndex - startIndex).ToString());

					journalItems.Add(journalItem);
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
		}
	}
}