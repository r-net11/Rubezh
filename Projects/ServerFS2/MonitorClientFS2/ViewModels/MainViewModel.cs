using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using FS2Api;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using MonitorClientFS2.ViewModels;

namespace MonitorClientFS2
{
	public class MainViewModel : BaseViewModel
	{
		//List<OldMonitoringDevice> devicesLastRecord;
		MonitoringProcessor MonitoringProcessor;

		public MainViewModel()
		{
			StartMonitoringCommand = new RelayCommand(OnStartMonitoring);
			StopMonitoringCommand = new RelayCommand(OnStopMonitoring);
			TestCommand = new RelayCommand(OnTest);
			ReadStatesCommand = new RelayCommand(OnReadStates);

			DevicesViewModel = new DevicesViewModel();

			//devicesLastRecord = new List<OldMonitoringDevice>();
			//JournalItems = new ObservableCollection<FSJournalItem>(DBJournalHelper.GetJournalItems(new Guid()));
			JournalItems = new ObservableCollection<FS2JournalItem>();
			MonitoringProcessor = new MonitoringProcessor();

			MonitoringDevice.NewJournalItem += new Action<FS2JournalItem>(ShowNewItem);
			MonitoringDevice.JournalItems += new Action<List<FS2JournalItem>>(ShowNewItems);

			//foreach (var device in ConfigurationManager.DeviceConfiguration.Devices)
			//{
			//    if (device.Driver.IsPanel)
			//    {
			//        try
			//        {
			//            devicesLastRecord.Add(new MonitoringDevice(device));
			//        }
			//        catch
			//        {
			//            Trace.Write("Ошибка при считывании последней записи");
			//        }
			//    }
			//}
			//OnStartMonitoring();
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
			MonitoringProcessor.StartMonitoring();
		}

		public RelayCommand StopMonitoringCommand { get; private set; }
		void OnStopMonitoring()
		{
			MonitoringProcessor.StopMonitoring();
		}

		public RelayCommand TestCommand { get; private set; }
		void OnTest()
		{
			MonitoringProcessor.WriteStats();
		}

		public RelayCommand ReadStatesCommand { get; private set; }
		void OnReadStates()
		{
			var deviceStatesManager = new DeviceStatesManager();
			deviceStatesManager.GetStates();
		}
	}
}