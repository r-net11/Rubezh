using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Input;
using Common.GK;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace DiagnosticsModule.ViewModels
{
	public class JournalTestViewModel : DialogViewModel
	{
		public JournalTestViewModel()
		{
            AddCommand = new RelayCommand(OnAdd);
            SelectCommand = new RelayCommand(OnSelect);
            JournalRecords = new ObservableCollection<Journal>();
		}

        public ObservableCollection<Journal> JournalRecords { get; private set; }

        public RelayCommand AddCommand { get; private set; }
        void OnAdd()
        {
            using (var dataContext = ConnectionManager.CreateGKDataContext())
            {
                var journal = new Journal();
                journal.DateTime = DateTime.Now;
                journal.ObjectUID = Guid.NewGuid();
                journal.GKObjectNo = 1;
                journal.Description = "Event Description";
                dataContext.Journal.InsertOnSubmit(journal);
                dataContext.SubmitChanges();
            }
        }

        public RelayCommand SelectCommand { get; private set; }
        void OnSelect()
        {
            using (var dataContext = ConnectionManager.CreateGKDataContext())
            {
                var query = "SELECT * FROM Journal";
                var result = dataContext.ExecuteQuery<Journal>(query);

                JournalRecords.Clear();
                foreach (var journalRecord in result)
                {
                    JournalRecords.Add(journalRecord);
                }
                var journalRecordsCount = JournalRecords.Count();
                Trace.WriteLine("Journal Count = " + journalRecordsCount.ToString());
            }
        }
	}
}