using FiresecAPI.Models;
using FiresecService.DatabaseConverter;
using System;
using System.Threading;
using System.ServiceModel;

namespace FiresecService
{
    public static class DatabaseHelper
    {
        public static void AddJournalRecord(JournalRecord journalRecord)
        {
            using (var dataContext = new FiresecDbConverterDataContext())
            {
                try
                {
                    dataContext.JournalRecords.InsertOnSubmit(journalRecord);
                    dataContext.SubmitChanges();
                }catch{};
            }
        }

        public static void AddInfoMessage(string userName, string mesage)
        {
            var journalRecord = new JournalRecord()
            {
                DeviceTime = DateTime.Now,
                SystemTime = DateTime.Now,
                StateType = StateType.Info,
                Description = mesage,
                User = userName
            };
            AddJournalRecord(journalRecord);

            Thread thread = new Thread(new ParameterizedThreadStart(Send));
            thread.Start(journalRecord);
        }

        static void Send(object obj)
        {
            lock (FiresecService.locker)
            {
                var journalRecord = obj as JournalRecord;
                CallbackManager.OnNewJournalRecord(journalRecord);
            }
        }
    }
}
