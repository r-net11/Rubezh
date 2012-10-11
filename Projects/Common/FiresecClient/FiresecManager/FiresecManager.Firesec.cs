using Firesec;
using System;
using Common;

namespace FiresecClient
{
    public partial class FiresecManager
    {
        public static FiresecDriver FiresecDriver { get; private set; }
        static int lastJournalNo;

        public static void InitializeFiresecDriver(string FS_Address, int FS_Port, string FS_Login, string FS_Password)
        {
            try
            {
                lastJournalNo = FiresecService.FiresecService.GetJournalLastId().Result;
                FiresecDriver = new FiresecDriver(lastJournalNo, FS_Address, FS_Port, FS_Login, FS_Password);
            }
            catch (Exception e)
            {
                Logger.Error(e, "FiresecManager.InitializeFiresecDriver");
                LoadingErrors.AppendLine(e.Message);
            }
        }

        public static void SynchrinizeJournal()
        {
            try
            {
                var journalRecords = FiresecDriver.Watcher.SynchrinizeJournal(lastJournalNo);
                if (journalRecords.Count > 0)
                {
                    FiresecService.AddJournalRecords(journalRecords);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "FiresecManager.SynchrinizeJournal");
                LoadingErrors.AppendLine(e.Message);
            }
        }
    }
}