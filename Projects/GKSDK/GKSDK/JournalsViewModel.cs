using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using RubezhAPI.GK;
using Infrastructure.Common.Windows.Windows.ViewModels;
using RubezhAPI.Journal;
using Infrastructure;
using RubezhClient;

namespace GKSDK
{
	public class JournalsViewModel : BaseViewModel
	{
		public JournalsViewModel()
		{
			JournalItems = new ObservableCollection<JournalItemViewModel>();
		}

		public ObservableCollection<JournalItemViewModel> JournalItems { get; private set; }

		public void OnNewJournalItems(List<JournalItem> journalItems)
		{
			foreach (var journalItem in journalItems)
			{
				var journalItemViewModel = new JournalItemViewModel(journalItem);
				JournalItems.Add(journalItemViewModel);
			}
		}

	}
}