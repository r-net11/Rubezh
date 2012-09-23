using System.ServiceModel;
using FiresecAPI;
using System.Collections.Generic;
using FiresecAPI.Models;
using System.Windows;
using System;
using Common;

namespace FiresecClient
{
	[CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Single)]
	public class FiresecEventSubscriber : IFiresecCallback
	{
        public void NewJournalRecords(List<JournalRecord> journalRecords)
        {
            MessageBox.Show("NewJournalRecords");
            //FiresecCallbackService.Current.NewJournalRecords(journalRecords);

            SafeOperationCall(() =>
            {
                foreach (var journalRecord in journalRecords)
                {
                    if (NewJournalRecordEvent != null)
                        NewJournalRecordEvent(journalRecord);
                }
            });
        }

        public static event Action<JournalRecord> NewJournalRecordEvent;

        static void SafeOperationCall(Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                Logger.Error(e, "Исключение при вызове FiresecCallbackService.SafeOperationCall");
            }
        }

		public void Ping()
		{
		}
	}
}