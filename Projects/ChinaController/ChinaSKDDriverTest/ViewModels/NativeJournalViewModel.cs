using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using ChinaSKDDriver;
using ChinaSKDDriverAPI;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using ChinaSKDDriverNativeApi;
using System.Threading;
using System.Runtime.InteropServices;
using FiresecAPI.Journal;

namespace ControllerSDK.ViewModels
{
	public class NativeJournalViewModel : BaseViewModel
	{
		public NativeJournalViewModel()
		{
			JournalItems = new ObservableCollection<JournalItemViewModel>();
			Wrapper.NewJournalItem +=new Action<SKDJournalItem>(AddJournalItem);
		}

		void AddJournalItem(SKDJournalItem journalItem)
		{
			var journalItemViewModel = new JournalItemViewModel(journalItem);
			Dispatcher.BeginInvoke(new Action(() =>
			{
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