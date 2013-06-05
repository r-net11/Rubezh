using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using FS2Api;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using MonitorClientFS2.ViewModels;
using ServerFS2.Processor;
using System.Collections;
using System.Diagnostics;

namespace MonitorClientFS2
{
	public class MainViewModel : BaseViewModel
	{
		public MainViewModel()
		{
			StartMonitoringCommand = new RelayCommand(OnStartMonitoring);
			StopMonitoringCommand = new RelayCommand(OnStopMonitoring);
			TestCommand = new RelayCommand(OnTest);
			ReadStatesCommand = new RelayCommand(OnReadStates);

			DevicesViewModel = new DevicesViewModel();

			JournalItems = new ObservableCollection<FS2JournalItem>();

			MainManager.NewJournalItem += new Action<FS2JournalItem>(ShowNewItem);
			//MainManager.StartMonitoring();
			
		}

		public DevicesViewModel DevicesViewModel { get; private set; }

		void ShowNewItem(FS2JournalItem journalItem)
		{
			Dispatcher.Invoke(new Action(() =>
			{
				JournalItems.Insert(0, journalItem);
			}));
		}

		void ShowNewItems(List<FS2JournalItem> journalItems)
		{
			journalItems.ForEach(x => ShowNewItem(x));
		}

		ObservableCollection<FS2JournalItem> _journalItems;
		public ObservableCollection<FS2JournalItem> JournalItems
		{
			get { return _journalItems; }
			set
			{
				_journalItems = value;
				OnPropertyChanged("JournalItems");
			}
		}

		FS2JournalItem _selectedJournalItem;
		public FS2JournalItem SelectedJournalItem
		{
			get { return _selectedJournalItem; }
			set
			{
				_selectedJournalItem = value;
				OnPropertyChanged("SelectedJournalItem");
			}
		}

		public RelayCommand StartMonitoringCommand { get; private set; }
		void OnStartMonitoring()
		{
			MainManager.StartMonitoring();
		}

		public RelayCommand StopMonitoringCommand { get; private set; }
		void OnStopMonitoring()
		{
			MainManager.StopMonitoring();
		}

		public RelayCommand TestCommand { get; private set; }
		void OnTest()
		{
			var stateBytes = new List<byte>() { 1, 2 };
			var bitArray = new BitArray(stateBytes.ToArray());
			for (int i = 0; i < 16; i++)
			{
				var isBitSetted = bitArray[i];
				Trace.WriteLine(i + "-" + isBitSetted);
			}
			//MonitoringProcessor.WriteStats();
		}

		public RelayCommand ReadStatesCommand { get; private set; }
		void OnReadStates()
		{
			//var deviceStatesManager = new DeviceStatesManager();
			//deviceStatesManager.GetStates();
		}
	}
}