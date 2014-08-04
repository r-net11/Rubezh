using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using ChinaSKDDriver;
using ChinaSKDDriverAPI;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using ChinaSKDDriverNativeApi;

namespace ControllerSDK.ViewModels
{
	public class JournalViewModel : BaseViewModel
	{
		public JournalViewModel()
		{
			JournalItems = new ObservableCollection<JournalItemViewModel>();
			Wrapper.NewJournalItem += new Action<SKDJournalItem>(Wrapper_NewJournalItem);
		}

		void Wrapper_NewJournalItem(SKDJournalItem journalItem)
		{
			Dispatcher.BeginInvoke(new Action(() =>
			{
				var journalItemViewModel = new JournalItemViewModel(journalItem);
				JournalItems.Add(journalItemViewModel);
			}));
		}

		public ObservableCollection<JournalItemViewModel> JournalItems { get; private set; }

		JournalItemViewModel _selectedJournalItem;
		public JournalItemViewModel SelectedJournalItem
		{
			get { return _selectedJournalItem; }
			set
			{
				_selectedJournalItem = value;
				OnPropertyChanged(() => SelectedJournalItem);
			}
		}
	}
}