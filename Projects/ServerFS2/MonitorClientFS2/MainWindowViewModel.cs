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
			JournalItems = new ObservableCollection<JournalItem>();
			lastRecord = new Dictionary<Device, int>();
			foreach (var device in ConfigurationManager.DeviceConfiguration.Devices)
			{
				if (device.Driver.DriverType == DriverType.Rubezh_2AM)
				{
					foreach (var journalItem in JournalHelper.GetJournalItems(device, JournalHelper.GetLastJournalItemId(device), JournalHelper.GetLastJournalItemId(device) - 1))
						JournalItems.Add(journalItem);
					lastRecord.Add(device, JournalHelper.GetLastJournalItemId(device));
				}
			}

			autoResetEvent = new AutoResetEvent(false);
			StartMonitoringCommand = new RelayCommand(OnStartMonitoring);
			StopMonitoringCommand = new RelayCommand(OnStopMonitoring);

			StartMonitoring();
		}

		bool stop = false;

		AutoResetEvent autoResetEvent;

		Thread thread;

		Dictionary<Device, int> lastRecord;

		public void StartMonitoring()
		{
			stop = false;
			thread = new Thread(new ThreadStart(() =>
			{
				Dispatcher.CurrentDispatcher.Invoke(new Action(() =>
				{
					Trace.WriteLine("Начинаю мониторинг");
					int lastDisplayedRecord;
					int lastDeviceRecord;
					while (!stop)
					{
						autoResetEvent.WaitOne(1000);
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
									foreach (var journalItem in JournalHelper.GetJournalItems(device, lastDeviceRecord, lastDisplayedRecord + 1))
										Dispatcher.Invoke(new Action(() =>
										{
											JournalItems.Add(journalItem);
										}));

									lastRecord.Remove(device);
									lastRecord.Add(device, lastDeviceRecord);
								}
								else
								{
									Trace.WriteLine("Новых записей нет");
								}
							}
						}
					}
				}));
			}));

			thread.IsBackground = true;
			thread.Start();
		}

		private void StopMonitoring()
		{
			stop = true;
			Trace.WriteLine("Останавливаю мониторинг");
		}

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

		private ObservableCollection<JournalItem> _journalItems;

		public ObservableCollection<JournalItem> JournalItems
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

		public RelayCommand StartMonitoringCommand { get; private set; }

		private void OnStartMonitoring()
		{
			if (stop)
			{
				StartMonitoring();
			}
		}

		public RelayCommand StopMonitoringCommand { get; private set; }

		private void OnStopMonitoring()
		{
			StopMonitoring();
		}
	}
}