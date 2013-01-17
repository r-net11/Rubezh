using System.Collections.Generic;
using Common.GK;
using FiresecClient;

namespace GKModule
{
    public partial class Watcher
    {
        public void ReadMissingJournalItems()
        {
            var gkIpAddress = XManager.GetIpAddress(GkDatabase.RootDevice);
            var localLastDBNo = GKDBHelper.GetLastGKID(gkIpAddress);
            if (localLastDBNo == -1)
                return;
            var remoteLastId = GetLastId();
            if (remoteLastId == -1)
                return;
            if (remoteLastId > localLastDBNo)
            {
                StartProgress("Синхронизация журнала", remoteLastId - localLastDBNo);
                SyncLocalAndRemote(localLastDBNo, remoteLastId);
                StopProgress();
                LastId = remoteLastId;
            }
        }

        void SyncLocalAndRemote(int startIndex, int endIndex)
        {
            var journalItems = new List<JournalItem>();
            for (int index = startIndex; index <= endIndex; index++)
            {
                var journalItem = ReadJournal(index);
                if (journalItem != null)
                {
                    DoProgress((index - startIndex).ToString() + " из " + (endIndex - startIndex).ToString());

                    journalItems.Add(journalItem);
                    if (journalItems.Count > 100)
                    {
                        GKDBHelper.AddMany(journalItems);
                        journalItems = new List<JournalItem>();
                    }
                }
            }
            if (journalItems.Count > 0)
            {
                GKDBHelper.AddMany(journalItems);
            }
        }
    }
}