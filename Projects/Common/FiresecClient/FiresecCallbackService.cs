using System;
using FiresecAPI;
using FiresecAPI.Models;

namespace FiresecClient
{
    public class FiresecCallbackService : IFiresecCallbackService
    {
        public void NewJournalRecord(JournalRecord journalRecord)
        {
            if (NewJournalRecordEvent != null)
                NewJournalRecordEvent(journalRecord);
        }

        public void ConfigurationChanged()
        {
            if (ConfigurationChangedEvent != null)
                ConfigurationChangedEvent();
        }

        public bool Progress(int stage, string comment, int percentComplete, int bytesRW)
        {
            if (ProgressEvent != null)
                return ProgressEvent(stage, comment, percentComplete, bytesRW);
            return true;
        }

        public static event Action ConfigurationChangedEvent;
        public static event Func<int, string, int, int, bool> ProgressEvent;
        public static event Action<JournalRecord> NewJournalRecordEvent;
    }
}