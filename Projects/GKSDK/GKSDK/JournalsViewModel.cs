using System;
using System.Collections.Generic;
using System.Windows.Threading;
using FiresecAPI.Models;
using System.Collections.ObjectModel;
using System.Windows;
using Common.GK;

namespace GKSDK
{
    public class JournalsViewModel : BaseViewModel
    {
        public JournalsViewModel()
        {
			JournalItems = new ObservableCollection<JournalItem>();
			//ItvManager.NewJournalRecord += new Action<JournalRecord>((x) => { SafeCall(() => { OnNewJournalRecord(x); }); });
        }

		public void SafeCall(Action action)
		{
			if (Application.Current != null && Application.Current.Dispatcher != null)
				Application.Current.Dispatcher.BeginInvoke(action);
		}

		void OnNewJournalRecord(JournalItem journalItems)
		{
			JournalItems.Add(journalItems);
		}

		public ObservableCollection<JournalItem> JournalItems { get; private set; }
    }
}