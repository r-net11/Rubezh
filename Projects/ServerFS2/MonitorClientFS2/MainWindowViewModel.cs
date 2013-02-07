using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;
using ClientFS2;
using ClientFS2.ViewModels;
using FiresecAPI.Models;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using ServerFS2;

namespace MonitorClientFS2
{
	public class MainWindowViewModel : BaseViewModel
	{
		public MainWindowViewModel()
		{
			ConfigurationManager.Load();
			DevicesViewModel = new ObservableCollection<DeviceViewModel>();
			foreach (var device in ConfigurationManager.DeviceConfiguration.Devices)
			{
				DevicesViewModel.Add(new DeviceViewModel(device));
			}
			JournalItems = new List<JournalItem>();
			lastRecord = new Dictionary<Device, int>();
			foreach (var device in ConfigurationManager.DeviceConfiguration.Devices)
			{
				if (device.Driver.DriverType == DriverType.Rubezh_2AM)
				{
					foreach (var journalItem in JournalHelper.GetLast100JournalItems(device))
						JournalItems.Add(journalItem);
					lastRecord.Add(device, JournalHelper.GetLastJournalItemId(device));
				}
			}

			thread = new Thread(new ThreadStart(() =>
			{
				StartMonitoring();
			}));

			thread.IsBackground = true;
			thread.Start();

			GetDevicesCommand = new RelayCommand(OnGetDevices);
			ReadJournalCommand = new RelayCommand(OnReadJournal);
			StopMonitoringCommand = new RelayCommand(OnStopMonitoring);
		}

		public void StartMonitoring()
		{
			Dispatcher.CurrentDispatcher.Invoke(new Action(() =>
			{
				int lastDisplayedRecord;
				int lastDeviceRecord;
				while (true)
				{
					Thread.Sleep(TimeSpan.FromMilliseconds(1000));
					foreach (var device in ConfigurationManager.DeviceConfiguration.Devices)
					{
						if (device.Driver.DriverType == DriverType.Rubezh_2AM)
						{
							lastRecord.TryGetValue(device, out lastDisplayedRecord);
							lastDeviceRecord = JournalHelper.GetLastJournalItemId(device);
							if (lastDeviceRecord != lastDisplayedRecord)
							{
								Trace.WriteLine("Дочитываю записи с " + lastDisplayedRecord.ToString() +
									" до " + JournalHelper.GetLastJournalItemId(device).ToString());
								foreach (var journalItem in JournalHelper.GetNewJournalItems(device, lastDisplayedRecord))
									JournalItems.Add(journalItem);
								lastRecord.Remove(device);
								lastRecord.Add(device, lastDeviceRecord);
								OnPropertyChanged("JournalItems");
							}
							else
							{
								Trace.WriteLine("Новых записей нет");
							}
						}
					}
				}
			}));
		}

		Thread thread;

		Dictionary<Device, int> lastRecord;

		private ObservableCollection<DeviceViewModel> _devicesViewModel;

		public ObservableCollection<DeviceViewModel> DevicesViewModel
		{
			get { return _devicesViewModel; }
			set
			{
				_devicesViewModel = value;
				OnPropertyChanged("devicesViewModel");
			}
		}

		private List<JournalItem> _journalItems;

		public List<JournalItem> JournalItems
		{
			get { return _journalItems; }
			set
			{
				_journalItems = value;
				OnPropertyChanged("JournalItems");
			}
		}

		private DeviceViewModel _selectedDeviceViewModel;

		public DeviceViewModel SelectedDeviceViewModel
		{
			get { return _selectedDeviceViewModel; }
			set
			{
				_selectedDeviceViewModel = value;
				OnPropertyChanged("SelectedDeviceViewModel");
			}
		}

		public RelayCommand GetDevicesCommand { get; private set; }

		private void OnGetDevices()
		{
		}

		public RelayCommand ReadJournalCommand { get; private set; }

		private void OnReadJournal()
		{
			//thread.Suspend();
		}

		public RelayCommand StopMonitoringCommand { get; private set; }

		private void OnStopMonitoring()
		{
			thread.Suspend();
			Trace.WriteLine("Мониторинг остановлен");
		}
	}
}