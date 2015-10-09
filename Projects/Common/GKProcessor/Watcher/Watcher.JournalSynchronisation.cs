using System;
using FiresecAPI;
using FiresecClient;
using RubezhDAL;

namespace GKProcessor
{
	public partial class Watcher
	{
		public bool ReadMissingJournalItems()
		{
			var gkIpAddress = GKManager.GetIpAddress(GkDatabase.RootDevice);
			var localLastDBNo = -1;
            using (var skdDatabaseService = new RubezhDAL.DataClasses.DbService())
			{
				localLastDBNo = skdDatabaseService.GKMetadataTranslator.GetLastJournalNo(gkIpAddress);
			}
			if (localLastDBNo == -1)
			{
				return true;
			}
			var remoteLastId = GetLastId();
			if (remoteLastId == -1)
			{
				return false;
			}
			using (var skdDatabaseService = new RubezhDAL.DataClasses.DbService())
			{
				skdDatabaseService.GKMetadataTranslator.SetLastJournalNo(gkIpAddress, remoteLastId);
			}
			if (remoteLastId > localLastDBNo)
			{
				if (remoteLastId - localLastDBNo > 1000)
					localLastDBNo = remoteLastId - 1000;

				var progressCallback = GKProcessorManager.StartProgress("Синхронизация журнала ГК " + gkIpAddress, "", remoteLastId - localLastDBNo, true, GKProgressClientType.Monitor);

				using (var gkLifecycleManager = new GKLifecycleManager(GkDatabase.RootDevice, "Синхронизация журнала"))
				{
					for (int index = localLastDBNo; index <= remoteLastId; index++)
					{
						gkLifecycleManager.Progress(index - localLastDBNo + 1, remoteLastId - localLastDBNo);

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
				}

				GKProcessorManager.StopProgress(progressCallback);
				LastId = remoteLastId;
			}
			return true;
		}
	}
}