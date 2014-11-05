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


			return true;


			var gkIpAddress = GKManager.GetIpAddress(GkDatabase.RootDevice);
			var localLastDBNo = -1;
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
						AddJournalItem(journaParser.JournalItem);
					}
				}

				GKProcessorManager.StopProgress(progressCallback);
				LastId = remoteLastId;
			}
			return true;
		}
	}
}