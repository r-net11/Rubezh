using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using GKProcessor;
using GKProcessor.Events;
using Infrastructure.Common.Services;
using Infrastructure.Common.Windows.ViewModels;
using XFiresecAPI;

namespace GKSDK
{
	public class JournalsViewModel : BaseViewModel
	{
		public JournalsViewModel()
		{
			JournalItems = new ObservableCollection<JournalItem>();
			ServiceFactoryBase.Events.GetEvent<NewXJournalEvent>().Subscribe(OnNewJournalItems);
		}

		public void SafeCall(Action action)
		{
			if (Application.Current != null && Application.Current.Dispatcher != null)
				Application.Current.Dispatcher.BeginInvoke(action);
		}

		void OnNewJournalItems(List<JournalItem> journalItems)
		{
			//SafeCall(() => { OnNewJournalRecord(x); });
			foreach (var journalItem in journalItems)
			{
				JournalItems.Add(journalItem);
			}
		}

		public ObservableCollection<JournalItem> JournalItems { get; private set; }
	}
}