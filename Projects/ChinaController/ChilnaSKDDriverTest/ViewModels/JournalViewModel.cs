using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Infrastructure.Common.Windows.ViewModels;
using Infrastructure.Common;
using ChinaSKDDriverNativeApi;
using System.Runtime.InteropServices;
using ChinaSKDDriver;
using ControllerSDK.Views;
using ChinaSKDDriverAPI;
using System.Threading;
using System.Diagnostics;
using System.Windows;

namespace ControllerSDK.ViewModels
{
	public class JournalViewModel : BaseViewModel
	{
		public JournalViewModel()
		{
			GetConnectionStatusCommand = new RelayCommand(OnConnectionStatus);
			JournalItems = new ObservableCollection<JournalItemViewModel>();
			MainViewModel.Wrapper.NewJournalItem += new Action<SKDJournalItem>(Wrapper_NewJournalItem);
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