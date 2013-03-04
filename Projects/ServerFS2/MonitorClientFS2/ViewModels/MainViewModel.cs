using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Windows.Threading;
using FiresecAPI.Models;
using Infrastructure.Common;
using Infrastructure.Common.Windows.ViewModels;
using MonitorClientFS2.ViewModels;
using ServerFS2;
using ServerFS2.DataBase;

namespace MonitorClientFS2
{
	public class MainViewModel : BaseViewModel
	{
		public MainViewModel()
		{
			StartMonitoringCommand = new RelayCommand(OnStartMonitoring);
			StopMonitoringCommand = new RelayCommand(OnStopMonitoring);
			TestCommand = new RelayCommand(OnTest);

			DevicesViewModel = new DevicesViewModel();

			devicesLastRecord = new Dictionary<Device, int>();
			devicesLastSecRecord = new Dictionary<Device, int>();
			Guid guid = new Guid();
			JournalItems = new ObservableCollection<FSJournalItem>(DBJournalHelper.GetJournalItems(guid));

			foreach (var device in ConfigurationManager.DeviceConfiguration.Devices)
			{
				if (device.Driver.IsPanel)
				{
					try
					{
						devicesLastRecord.Add(device, DBJournalHelper.GetLastId(guid));
					}
					catch { }

					if (device.Driver.DriverType == DriverType.Rubezh_2OP)
					{
						try
						{
							devicesLastSecRecord.Add(device, DBJournalHelper.GetLastSecId(guid));
						}
						catch { }
					}
				}
			}

			autoResetEvent = new AutoResetEvent(false);
			StartMonitoring();
		}

		public DevicesViewModel DevicesViewModel { get; private set; }

		bool stop = false;
		AutoResetEvent autoResetEvent;
		Thread monitoringThread;
		Dictionary<Device, int> devicesLastRecord;
		Dictionary<Device, int> devicesLastSecRecord;

		public void StartMonitoring()
		{
			stop = false;
			monitoringThread = new Thread(new ThreadStart(() =>
			{
				Dispatcher.CurrentDispatcher.Invoke(new Action(() =>
				{
					Trace.WriteLine("Начинаю мониторинг");
					int lastDisplayedRecord;
					int lastDeviceRecord;

					int lastDisplayedSecRecord;
					int lastDeviceSecRecord;

					while (!stop)
					{
						autoResetEvent.WaitOne(1000);
						foreach (var device in ConfigurationManager.DeviceConfiguration.Devices)
						{
							if (device.Driver.IsPanel)
							{
								devicesLastRecord.TryGetValue(device, out lastDisplayedRecord);
								lastDeviceRecord = JournalHelper.GetLastJournalItemId(device);
								if (lastDeviceRecord != lastDisplayedRecord)
								{
									ReadNewItems(lastDisplayedRecord, lastDeviceRecord, device);
								}

								if (device.Driver.DriverType == DriverType.Rubezh_2OP)
								{
									devicesLastSecRecord.TryGetValue(device, out lastDisplayedSecRecord);
									lastDeviceSecRecord = JournalHelper.GetLastSecJournalItemId2Op(device);
									if (lastDeviceSecRecord != lastDisplayedSecRecord)
									{
										ReadNewSecItems(lastDisplayedSecRecord, lastDeviceSecRecord, device);
									}
								}
							}
						}
					}
					autoResetEvent.Set();
				}));
			}));

			monitoringThread.IsBackground = true;
			monitoringThread.Start();
		}

		private void ReadNewSecItems(int lastDisplayedSecRecord, int lastDeviceSecRecord, Device device)
		{
			Trace.Write("Дочитываю охранные записи с " +
											  lastDisplayedSecRecord.ToString() +
											  " до " +
											  lastDeviceSecRecord.ToString());
			var newItems = JournalHelper.GetSecJournalItems2Op(device, lastDeviceSecRecord, lastDisplayedSecRecord + 1);
			foreach (var journalItem in newItems)
			{
				AddToJournalObservable(journalItem);
				DBJournalHelper.AddJournalItem(journalItem);
				Trace.Write(".");
			}
			Trace.WriteLine(" дочитал");
			devicesLastRecord.Remove(device);
			devicesLastRecord.Add(device, lastDeviceSecRecord);
			DBJournalHelper.SetLastSecId(new Guid(), lastDeviceSecRecord);
		}

		private void ReadNewItems(int lastDisplayedRecord, int lastDeviceRecord, Device device)
		{
			Trace.Write("Дочитываю записи с " +
										   lastDisplayedRecord.ToString() +
										   " до " +
										   lastDeviceRecord.ToString());
			var newItems = JournalHelper.GetJournalItems(device, lastDeviceRecord, lastDisplayedRecord + 1);
			foreach (var journalItem in newItems)
			{
				AddToJournalObservable(journalItem);
				DBJournalHelper.AddJournalItem(journalItem);
				Trace.Write(".");
			}
			Trace.WriteLine(" дочитал");
			devicesLastRecord.Remove(device);
			devicesLastRecord.Add(device, lastDeviceRecord);
			DBJournalHelper.SetLastId(new Guid(), lastDeviceRecord);
		}

		private void AddToJournalObservable(FSJournalItem journalItem)
		{
			Dispatcher.Invoke(new Action(() =>
			{
				JournalItems.Add(journalItem);
			}));
		}

		private void StopMonitoring()
		{
			Trace.WriteLine(monitoringThread.ThreadState.ToString());
			stop = true;
			Trace.WriteLine("Останавливаю мониторинг");
		}

		private ObservableCollection<FSJournalItem> _journalItems;

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

		public RelayCommand TestCommand { get; private set; }

		private void OnTest()
		{
		}
	}
}