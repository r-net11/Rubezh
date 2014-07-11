using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using ChinaSKDDriverAPI;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using ChinaSKDDriver;

namespace ControllerSDK.ViewModels
{
	public class JournalViewModel : BaseViewModel
	{
		public JournalViewModel()
		{
			GetConnectionStatusCommand = new RelayCommand(OnConnectionStatus);
			JournalItems = new ObservableCollection<JournalItemViewModel>();
			Wrapper.NewJournalItem += new Action<SKDJournalItem>(Wrapper_NewJournalItem);
		}

		void Wrapper_NewJournalItem(SKDJournalItem journalItem)
		{
			Trace.WriteLine("SKDJournalItem " + journalItem.DeviceDateTime.ToString());
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

		public RelayCommand GetConnectionStatusCommand { get; private set; }
		void OnConnectionStatus()
		{
			var result = MainViewModel.Wrapper.IsConnected();
			MessageBox.Show(result.ToString());
		}
	}
}