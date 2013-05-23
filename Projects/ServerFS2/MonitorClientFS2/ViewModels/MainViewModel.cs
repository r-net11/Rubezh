﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using MonitorClientFS2.ViewModels;
using ServerFS2.DataBase;

namespace MonitorClientFS2
{
	public class MainViewModel : BaseViewModel
	{
		List<OldMonitoringDevice> devicesLastRecord;
		MonitoringProcessor MonitoringProcessor;

		public MainViewModel()
		{
			StartMonitoringCommand = new RelayCommand(OnStartMonitoring);
			StopMonitoringCommand = new RelayCommand(OnStopMonitoring);
			TestCommand = new RelayCommand(OnTest);

			DevicesViewModel = new DevicesViewModel();

			devicesLastRecord = new List<OldMonitoringDevice>();
			//JournalItems = new ObservableCollection<FSJournalItem>(DBJournalHelper.GetJournalItems(new Guid()));
			JournalItems = new ObservableCollection<FSJournalItem>();
			MonitoringProcessor = new MonitoringProcessor();

			MonitoringDevice.OnNewItems += new Action<FSJournalItem>(ShowNewItem);

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

		void ShowNewItem(FSJournalItem journalItem)
		{
			Dispatcher.Invoke(new Action(() =>
			{
				JournalItems.Add(journalItem);
			}));
		}

		ObservableCollection<FSJournalItem> _journalItems;
		public ObservableCollection<FSJournalItem> JournalItems
		{
			get { return _journalItems; }
			set
			{
				_journalItems = value;
				OnPropertyChanged("JournalItems");
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
			;
		}
	}
}