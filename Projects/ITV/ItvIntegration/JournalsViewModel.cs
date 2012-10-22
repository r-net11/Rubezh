using System;
using System.Collections.Generic;
using System.Windows.Threading;
using FiresecAPI.Models;
using FiresecClient.Itv;

namespace ItvIntegration
{
    public class JournalsViewModel : BaseViewModel
    {
        public JournalsViewModel()
        {
            JournalRecords = new List<JournalRecord>();
            ItvManager.NewJournalRecord += new Action<JournalRecord>(OnNewJournalRecord);
        }

        void OnNewJournalRecord(JournalRecord journalRecord)
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => { JournalRecords.Add(journalRecord); }));
            OnPropertyChanged("JournalRecords");
        }

        public List<JournalRecord> JournalRecords { get; private set; }
    }
}