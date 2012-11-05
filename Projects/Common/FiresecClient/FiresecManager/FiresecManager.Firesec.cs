using System;
using Common;
using Firesec;
using FiresecAPI;

namespace FiresecClient
{
    public partial class FiresecManager
    {
        public static FiresecDriver FiresecDriver { get; private set; }

        static public OperationResult<bool> InitializeFiresecDriver(string FS_Address, int FS_Port, string FS_Login, string FS_Password, bool isPing)
        {
            try
            {
                FiresecDriver = new FiresecDriver();
                return FiresecDriver.Connect(FS_Address, FS_Port, FS_Login, FS_Password, isPing);
            }
            catch (Exception e)
            {
                Logger.Error(e, "FiresecManager.InitializeFiresecDriver");
                AddLoadingError(e);
                return new OperationResult<bool>(e.Message);
            }
        }

        public static void SynchrinizeJournal()
        {
            try
            {
                var result = FiresecService.FiresecService.GetJournalLastId();
                if (result.HasError)
                {
                    AddLoadingError("Ошибка при получении индекса последней записи с сервера");
                }

                var journalRecords = FiresecDriver.Watcher.SynchrinizeJournal(result.Result);
                if (journalRecords.Count > 0)
                {
                    FiresecService.AddJournalRecords(journalRecords);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "FiresecManager.SynchrinizeJournal");
                AddLoadingError(e);
            }
        }
    }
}